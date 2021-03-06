﻿using System.Collections.Generic;
using System.Linq;
using Nest;
using Newtonsoft.Json.Linq;
using Tests.Core.ManagedElasticsearch.Clusters;
using Tests.Domain;
using Tests.Framework.Integration;
using Tests.Framework.ManagedElasticsearch.Clusters;
using static Nest.Infer;

namespace Tests.QueryDsl.Joining.Nested
{
	/**
	* Nested query allows to query nested objects / docs (see {ref_current}/nested.html[nested mapping]).
	* The query is executed against the nested objects / docs as if they were indexed as separate
	* docs (they are, internally) and resulting in the root parent doc (or parent nested mapping).
	*
	* See the Elasticsearch documentation on {ref_current}/query-dsl-nested-query.html[nested query] for more details.
	*/
	public class NestedUsageTests : QueryDslUsageTestsBase
	{
		public NestedUsageTests(ReadOnlyCluster i, EndpointUsage usage) : base(i, usage) { }

		protected override object QueryJson => new
		{
			nested = new
			{
				_name = "named_query",
				boost = 1.1,
				query = new
				{
					terms = new Dictionary<string, object>
					{
						{ "curatedTags.name", new [] {"lorem", "ipsum"} }
					}
				},
				ignore_unmapped = true,
				path = "curatedTags",
				inner_hits = new
				{
					explain = true
				}
			}
		};

		protected override QueryContainer QueryInitializer => new NestedQuery
		{
			Name = "named_query",
			Boost = 1.1,
			InnerHits = new InnerHits { Explain = true },
			Path = Field<Project>(p => p.CuratedTags),
			Query = new TermsQuery
			{
				Field = Field<Project>(p => p.CuratedTags.First().Name),
				Terms = new[] { "lorem", "ipsum" }
			},
			IgnoreUnmapped = true
		};

		protected override QueryContainer QueryFluent(QueryContainerDescriptor<Project> q) => q
			.Nested(c => c
				.Name("named_query")
				.Boost(1.1)
				.InnerHits(i=>i.Explain())
				.Path(p=>p.CuratedTags)
				.Query(nq => nq
					.Terms(t => t
						.Field(f => f.CuratedTags.First().Name)
						.Terms("lorem", "ipsum")
					)
				)
				.IgnoreUnmapped()
			);

		protected override ConditionlessWhen ConditionlessWhen => new ConditionlessWhen<INestedQuery>(a => a.Nested)
		{
			q =>  q.Query = null,
			q =>  q.Query = ConditionlessQuery,
			q =>  q.Path = null,
		};
	}
}
