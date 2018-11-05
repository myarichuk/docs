using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents;

namespace Raven.Documentation.Samples.indexes.querying.graph
{
    public class Graph
    {
        public void RawQuery_Samples()
        {
            using (var store = new DocumentStore
            {
                Urls = new[] {"http://no_way_this_url_exists_and_if_does_it_is_a_major_wtf:8080"}
            })
            {
                //illustrates that single vertex in SELECT clause yields flat document list - the same as document queries
                #region Single_result_query

                using (var session = store.OpenSession())
                {
                    var stronglyTypedResults = session.Advanced.RawQuery<Dog>(@"match (Dogs as dogA)-[Likes]->(Dogs as friend)<-[Likes]-(Dogs as dogB)
                                                                                select friend").ToList();
                    //do stuff with stronglyTypedResults
                }

                #endregion

                //illustrates that multiple retrieved vertices yield multiple documents per row
                #region Multiple_results_query                   
                using (var session = store.OpenSession())
                {
                    var jsonResults = session.Advanced.RawQuery<JObject>(@"match (Dogs as dogA)-[Likes]->(Dogs as friend)<-[Likes]-(Dogs as dogB)").ToList();
                    
                    var stronglyTypedResults = jsonResults.Select(x => new
                                                            {
                                                                DogA = x["dogA"].ToObject<Dog>(),
                                                                DogB = x["dogB"].ToObject<Dog>(),
                                                                Friend = x["friend"].ToObject<Dog>()
                                                            });
                    //do stuff with stronglyTypedResults
                }
                #endregion

                //note - show 'Hobbit_Person_Class' in the article before this
                #region Query_parameter_usage

                using (var session = store.OpenSession())
                {
                    var results = session.Advanced.RawQuery<Person>(@"
                            match (People as son)-[Parents where Gender = $gender select Id]->(People as paternal where BornAt='Shire')
                            select son")
                        .AddParameter("gender", "Male")
                        .ToList();

                    //do stuff with results
                }                

                #endregion

                //note - show 'Hobbit_Ancestry_Class' in the article before this
                //note 2 - link to recursive graph queries article for more details after this
                #region Strongly_typed_ancestry_usage

                using (var session = store.OpenSession())
                {
                    var results = session.Advanced.RawQuery<HobbitAncestry>(@"
                        match (People as son)-recursive as ancestry ($min,$max, $type) { [Parents where Gender = 'Male' select Id]->(People as paternal where BornAt='Shire') } 
                        select ancestry.paternal.Name as PaternalAncestors, son.Name")
                        .AddParameter("min", 2)
                        .AddParameter("max", 3)
                        .AddParameter("type", "longest")
                        .ToList();
                }

                #endregion
              

                #region GraphQuery_with_full_query_syntax

                using (var session = store.OpenSession())
                {
                    var results = session.Advanced.RawQuery<JObject>(@"
                            match (Orders as o where id() = 'orders/828-A')-[Lines where ProductName in ('Spegesild', 'Chang') select Product]->(Products as p)                
                                    ").ToList();

                    //do something with 'resi;ts'

                    #endregion
                }
        }
    }

    public class Dog
    {
    }

    #region Hobbit_Person_Class
   
    public class Person
    {
        public class Parent
        {
            public string Id;
            public string Gender;
        }

        public Parent[] Parents;
        public string BornAt;
        public string Name;
    }

    #endregion

    #region Hobbit_Ancestry_Class
   
    public class HobbitAncestry
    {
        public string Name;
        public string[] PaternalAncestors;
    }

    #endregion
}
