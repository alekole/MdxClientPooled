﻿using System;
using System.Data;
using System.Linq;
using Dapper;
using MdxClientPooled;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicTyped.Data.Test
{
    
    
    /// <summary>
    ///This is a test class for CubeCommandTest and is intended
    ///to contain all CubeCommandTest Unit Tests
    ///</summary>
    [TestClass]
    public class CubeCommandTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void CustomParmsTest()
        {
            var connection = UnitTestHelpers.GetCapellaDataTestConnection();
            using (connection)
            {
                connection.Open();
                const string query = @"SELECT [Measures].[Response Computation] on 0,
                                [Organization].[Organization].&[407] on 1
                                FROM Report
                                WHERE ([Computation].[Computation].&[Mean],
                                       [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q2]
                                        )";

                var d = new DynamicParameters();
                d.Add("~[Measures].[Response Computation]", "Score", DbType.Double);

                dynamic x = connection.Query(query, d);

                foreach (var y in x)
                {
                    Assert.AreEqual(64.6481481481482, y.Score, .00000001);
                    break;
                }
            }
        }

        [TestMethod]
        public void DataSetTestWithColumnMap()
        {
            var dataSet = new DataSet();

            var connection = UnitTestHelpers.GetCapellaDataTestConnection();
            using (connection)
            {
                connection.Open();
                const string query = @"SELECT [Measures].[Response Computation] on 0,
                                [Organization].[Organization].&[407] on 1
                                FROM Report
                                WHERE ([Computation].[Computation].&[Mean],
                                       [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q2]
                                        )";


                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.Add(new MdxParameter("~[Measures].[Response Computation]", "Score"));
                    command.Parameters.Add(new MdxParameter("~[Organization].[Organization].[Level 05]", "Store"));
                    var dataAdapter = new MdxDataAdapter {SelectCommand = command};
                    dataAdapter.Fill(dataSet);
                }

                // Verify there is a table
                Assert.AreEqual(1, dataSet.Tables.Count);

                // Verify data
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    Assert.AreEqual("Chicago (IL)", item["Store"].ToString());
                    Assert.AreEqual(64.6481481481482, (double)item["Score"], .00000001);
                    break;
                }
            }
        }

        [TestMethod]
        public void NothingOnRowsTest()
        {
            var connection = UnitTestHelpers.GetCapellaDataTestConnection();
            using (connection)
            {
                connection.Open();

                const string query = @"SELECT [Measures].[Response Computation] on 0 FROM Report";

                var x = connection.Query(query);

                Assert.AreEqual(1, x.Count());
            }
        }

        [TestMethod]
        public void DataSetTestWithoutColumnMap()
        {
            var dataSet = new DataSet();

            var connection = UnitTestHelpers.GetCapellaDataTestConnection();
            using (connection)
            {
                connection.Open();
                const string query = @"SELECT [Measures].[Response Computation] on 0,
                                [Organization].[Organization].&[407] on 1
                                FROM Report
                                WHERE ([Computation].[Computation].&[Mean],
                                       [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q2]
                                        )";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    var dataAdapter = new MdxDataAdapter {SelectCommand = command};
                    dataAdapter.Fill(dataSet);
                }

                // Verify there is a table
                Assert.AreEqual(1, dataSet.Tables.Count);

                // Verify data
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    Assert.AreEqual("Chicago (IL)", item[0].ToString());
                    Assert.AreEqual(64.6481481481482, (double)item[1], .000000001);
                    break;
                }
            }
        }

        [TestMethod]
        public void DataTableTest()
        {
            var dataTable = new DataTable();

            var connection = UnitTestHelpers.GetCapellaDataTestConnection();
            using (connection)
            {
                connection.Open();
                const string query = @"SELECT [Measures].[Response Computation] on 0,
                                [Organization].[Organization].&[407] on 1
                                FROM Report
                                WHERE ([Computation].[Computation].&[Mean],
                                       [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q2]
                                        )";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.Add(new MdxParameter("~[Measures].[Response Computation]", "Score"));
                    command.Parameters.Add(new MdxParameter("~[Organization].[Organization].[Level 05]", "Store"));
                    using (var dataAdapter = new MdxDataAdapter())
                    {
                        dataAdapter.SelectCommand = command;
                        dataAdapter.Fill(dataTable);
                    }
                }

                // Verify data
                foreach (DataRow item in dataTable.Rows)
                {
                    Assert.AreEqual("Chicago (IL)", item[0].ToString());
                    Assert.AreEqual(64.6481481481482, (double)item[1], .000000001);
                    break;
                }
            }
        }

        [TestMethod]
        public void ParmTest()
        {
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"SELECT [Measures].[Response Computation] ON 0,
DESCENDANTS([Organization].[Organization].&[61],1)  on 1
FROM REPORT
WHERE ([Computation].[Computation].&[Mean], 
       [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q2],
	   @ReportPeriod
	   )";

                var parms = new DynamicParameters();
                parms.Add("@ReportPeriod", "[Report Period].[Report Period].&[November 2012]");
                parms.Add("~[Measures].[Response Computation]", "score");

                var x = connection.Query(query, parms).ToList();
                Assert.IsNotNull(x.First().score);
                Assert.AreEqual(36, x.Count());

            }
        }

        [TestMethod]
        public void NoResultsTest()
        {
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"SELECT [Measures].[Response Computation] ON 0,
NON EMPTY(DESCENDANTS([Organization].[Organization].&[61],1))  on 1
FROM REPORT
WHERE ([Computation].[Computation].&[Mean], 
       [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q2],
	   [Report Period].[Report Period].&[November 20123]
	   )";

                var x = connection.Query(query);
                Assert.AreEqual(0, x.Count());
                
            }
        }

        [TestMethod]
        public void ColumnMappingOrdinalOnlyTest()
        {
            using(var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"SELECT [Measures].[Response Computation] ON 0,
DESCENDANTS([Organization].[Organization].&[61],1) * [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q2] on 1
FROM REPORT
WHERE [Computation].[Computation].&[Mean]";
                
                var parms = new DynamicParameters();
                parms.Add("~0", "label");
                parms.Add("~2", "score");

                var x = connection.Query<StandardScore>(query, parms).ToList();
                var specificItem = x.First();
                Assert.AreEqual(36, x.Count, "item count");
                Assert.AreEqual(66.79, specificItem.Score, .01, "score");
                Assert.AreEqual("Mexico City", specificItem.Label, "label");
            }
        }

        [TestMethod]
        public void ColumnMappingMixingTildaAndOrdinalTest()
        {
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"SELECT [Measures].[Response Computation] ON 0,
DESCENDANTS([Organization].[Organization].&[61],1) * [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q2] on 1
FROM REPORT
WHERE [Computation].[Computation].&[Mean]";
                var parms = new DynamicParameters();
                parms.Add("~0", "label");
                parms.Add("~[Measures].[Response Computation]", "score");

                var x = connection.Query<StandardScore>(query, parms).ToList();
                var specificItem = x.First();
                Assert.AreEqual(36, x.Count, "item count");
                Assert.AreEqual(66.79, specificItem.Score, .01);
                Assert.AreEqual("Mexico City", specificItem.Label, "label");
            }
        }
        
        [TestMethod]
        public void ExecuteScalarTest()
        {
            object scalar;
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"SELECT [Measures].[Response Computation] on 0,
                                [Organization].[Organization].&[407] on 1
                                FROM Report
                                WHERE ([Computation].[Computation].&[Mean],
                                       [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q2]
                                        )";
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.Add(new MdxParameter("~[Measures].[Response Computation]", "Score"));
                    command.Parameters.Add(new MdxParameter("~[Organization].[Organization].[Level 05]", "Store"));
                    scalar = command.ExecuteScalar();
                }
            }
            Assert.AreEqual("Chicago (IL)", scalar.ToString());
        }

        [TestMethod]
        public void SettingDataTypeTest()
        {
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"WITH 
                            MEMBER DisplayScore AS
                            [Measures].[Response Computation]

                            MEMBER CalcScore AS
                            [Measures].[Response Computation]

                            SET EntitiesWithScores AS
                            ORDER(Filter([Employee].[Employee Guid].&[{2B10EE84-084E-1EEE-63C5-D54007BAF192}].siblings, [Measures].[Response Count]> 0), CalcScore, BDESC)


                            MEMBER [Rank] AS
                            RANK([Employee].[Employee Guid].currentmember, EntitiesWithScores, DisplayScore)

                            MEMBER MaxRank AS
                            MAX(EntitiesWithScores, [Rank])


                            SELECT {DisplayScore,  [Measures].[Response Computation], [Measures].[Response Count]} on 0,
                            ([Employee].[Employee Guid].&[{2B10EE84-084E-1EEE-63C5-D54007BAF192}]) on 1
                            FROM [Report]
                            WHERE (
		                               [Questionnaire].[Response Value Type].&[code],
                                       [Computation].[Computation].&[Mean], 
                                       [Organization].[Organization Hierarchy Name].&[Sales Demo],
                                       [Report Period].[Report Period].&[February 2012],
                                       [Report Period].[Report Period Type].&[3 Month Roll],
                                       [Organization].[Organization].&[356],
                                       [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Repurchase]
                                      )    ";

                var dp = new DynamicParameters();
                dp.Add("~0", "Label");
                dp.Add("~[Measures].[DisplayScore]", "Score", DbType.Double);
                dp.Add("~[Measures].[Response Computation]", "RawScore", DbType.Double);
                var actual = connection.Query<Metric>(query,dp).SingleOrDefault();
                Assert.IsNotNull(actual);
                Assert.AreEqual("77.2727272727273", actual.Score.ToString());
                Assert.AreEqual("{2B10EE84-084E-1EEE-63C5-D54007BAF192}", actual.Label);
                Assert.AreEqual("77.2727272727273", actual.RawScore.ToString());
            }
        }

        [TestMethod]
        public void GermanMetricTest()
        {
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            { 
                const string query = @"WITH 
                                                            MEMBER DisplayScore AS
                                                            ROUND([Measures].[Response Computation], 0)

                                                            MEMBER CalcScore AS
                                                            ROUND([Measures].[Response Computation], 0)

                                                            SET EntitiesWithScores AS
                                                            ORDER(Filter([Organization].[Organization].&[71].siblings, [Measures].[Response Count]> 0), CalcScore, BDESC)

                                                            MEMBER [Type] AS
                                                            IIF([Questionnaire].[Question Id].currentmember is [Questionnaire].[Question Id].[All], 'Category', 'Question')

                                                            MEMBER Label AS
                                                            [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.member_caption

                                                            MEMBER MdxValue AS
                                                            [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.uniquename
                                                   
                                                            MEMBER SqlValue AS
                                                            IIF([Type] = 'Category', [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.name, [Questionnaire].[Question Id].currentmember.name)

                                                            MEMBER ParentMdxValue AS
                                                            IIF([Type] = 'Category', null, [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.parent.uniquename)
                                                   
                                                            MEMBER ParentLabel AS
                                                            IIF([Type] = 'Category', NULL, [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.member_caption)                            

                                                            MEMBER [Rank] AS
                                                            RANK([Organization].[Organization].currentmember, EntitiesWithScores, DisplayScore)

                                                            MEMBER MaxRank AS
                                                            MAX(EntitiesWithScores, [Rank])

                                                            MEMBER ComparatorScore AS
                                                            ([Organization].[Organization].&[356].parent, DisplayScore)

                                                            MEMBER Range1 AS
                                                            IIF([Type] = 'Category', ([Organization].[Organization].currentmember.datamember, [Measures].[Organization Question Category Threshold Low Score]), ([Organization].[Organization].currentmember.datamember, [Measures].[Organization Question Threshold Low Score]))

                                                            MEMBER Range2 AS
                                                            IIF([Type] = 'Category', ([Organization].[Organization].currentmember.datamember, [Measures].[Organization Question Category Threshold High Score]), ([Organization].[Organization].currentmember.datamember, [Measures].[Organization Question Threshold High Score]))

                                                            MEMBER Range3 AS
                                                            IIF([Type] = 'Category', ([Organization].[Organization].currentmember.datamember, [Measures].[Organization Question Category Threshold Maximum Score]), ([Organization].[Organization].currentmember.datamember, [Measures].[Organization Question Threshold Maximum Score]))

                                                            SELECT {[Measures].[Label], DisplayScore, [Rank], MaxRank, [Measures].[MdxValue], [Measures].[SqlValue], [Measures].[Response Computation], [Measures].[Response Count], ParentLabel, ParentMdxValue, Range1, Range2, Range3 , [Measures].[ComparatorScore] } on 0,
                                                            ([Organization].[Organization].&[356], [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question Category]) on 1
                                                            FROM [Report]
                                                            WHERE (
                                                                       [Questionnaire].[Response Value Type].&[code],
                                                                       [Computation].[Computation].&[Mean], 
                                                                       [Organization].[Organization Hierarchy Name].&[Sales Demo],
                                                                       [Report Period].[Report Period].&[November 2012],
                                                                       [Report Period].[Report Period Type].&[3 Month Roll]
                                                                      )   ";
                connection.Open();
                var parms = new DynamicParameters();
                parms.Add("~2", "Label");
                parms.Add("~3", "Score");
                parms.Add("~8", "RawScore");
                parms.Add("~12", "Range1");
                parms.Add("~13", "Range2");
                parms.Add("~14", "Range3");
                parms.Add("~15", "ComparatorScore");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");

                var scores = connection.Query<Metric>(query, parms);
                Assert.IsTrue(scores.Any());
                
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        }


        [TestMethod]
        public void ExtraColumnsTest()
        {
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"WITH 
                                MEMBER DisplayScore AS
                                ROUND([Measures].[Response Computation], 0)

                                MEMBER CalcScore AS
                                ROUND([Measures].[Response Computation], 0)

                                SET EntitiesWithScores AS
                                ORDER(Filter([Organization].[Organization].&[407].siblings,[Measures].[Response Count]> 0), CalcScore, BDESC)

                                MEMBER [Type] AS
                                IIF([Questionnaire].[Question Id].currentmember is [Questionnaire].[Question Id].[All], 'Category', 'Question')

                                MEMBER ParentMdxValue AS
                                IIF([Type] = 'Category', null, [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.parent.uniquename)

                                MEMBER ParentLabel AS
                                IIF([Type] = 'Category', NULL, [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.member_caption)                            

                                MEMBER [Rank] AS
                                RANK([Organization].[Organization].currentmember, EntitiesWithScores, DisplayScore)

                                MEMBER MaxRank AS
                                MAX(EntitiesWithScores, [Rank])

                                MEMBER ComparatorScore AS
			                                ([Organization].[Organization].&[1357].parent, DisplayScore)

                                MEMBER Range1 AS
                                IIF([Type] = 'Category', ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Category Threshold Low Score]), ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Threshold Low Score]))

                                MEMBER Range2 AS
                                IIF([Type] = 'Category', ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Category Threshold High Score]), ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Threshold High Score]))

                                MEMBER Range3 AS
                                IIF([Type] = 'Category', ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Category Threshold Maximum Score]), ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Threshold Maximum Score]))

                                SELECT {DisplayScore, [Rank], MaxRank, [Measures].[Response Computation], [Measures].[Response Count], ParentLabel, ParentMdxValue, Range1, Range2, Range3 ,[Measures].[ComparatorScore] } on 0,
                                ([Organization].[Organization].&[407], [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question]) on 1
                                FROM [Report]
                                WHERE (
	                                    [Questionnaire].[Response Value Type].&[code],
	                                    [Computation].[Computation].&[Mean], 
	                                    [Organization].[Organization Hierarchy Name].&[Sales Demo],
	                                    [Report Period].[Report Period].&[November 2012],
	                                    [Report Period].[Report Period Type].&[3 Month Roll]
	                                    )";
                var parms = new DynamicParameters();
                parms.Add("~1", "Label");
                parms.Add("~1##UniqueName##", "MdxValue");
                parms.Add("~[Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question]##UniqueName##", "SqlValue");
                parms.Add("~2", "score");

                var x = connection.Query<Metric>(query, parms).ToList();
                
                Assert.AreEqual(23, x.Count(), "item count");
                var specificItem = x.SingleOrDefault(a => a.Label == "Product knowledge");
                Assert.IsNotNull(specificItem);
                Assert.AreEqual(76, specificItem.Score, "score");
                Assert.AreEqual("Product knowledge", specificItem.Label, "label");
                Assert.AreEqual("[Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q4]", specificItem.MdxValue, "mdx value");
                Assert.AreEqual("[Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[Q4]", specificItem.SqlValue, "sql value");

            }
        }

        [TestMethod]
        public void ColumnOnlyNullsOnEndTest()
        {
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"WITH
                MEMBER O AS 
                IIF([Measures].[Open] = NULL, 0, [Measures].[Open])

                MEMBER C AS
                IIF([Measures].[Closed] = NULL, 0, [Measures].[Closed])

                MEMBER New AS
                [Measures].[Collection Alert Distinct Count] - O - C

                SELECT {[Measures].[O] , [Measures].[C], new, [Measures].[Collection Count] } ON 0

                FROM Report
                WHERE (LASTPERIODS(30,StrToMember('[Date].[Date].&['+Format(Now(),""yyyy-MM-ddT00:00:00"")+']'))
                ,[Organization].[Organization].&[1442]
                ,[Questionnaire].[Questionnaire Version - Questionnaire - Question Category].[Questionnaire].&[1]&[1]
                )";

                var dp = new DynamicParameters();
                dp.Add("~[Measures].[O]", "Open");
                dp.Add("~[Measures].[New]", "New");
                dp.Add("~[Measures].[C]", "Closed");
                dp.Add("~[Measures].[Collection Count]", "Total");

                var result = connection.Query<AlertState>(query, dp);
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void ColumnOnlyTest()
        {
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"WITH
                
                MEMBER New AS
                [Measures].[Collection Alert Distinct Count] - [Measures].[Open] - [Measures].[Closed]

                SELECT {[Measures].[Open] , [Measures].[Closed], new, [Measures].[Collection Count] } ON 0

                FROM Report
                WHERE (LASTPERIODS(30,StrToMember('[Date].[Date].&['+Format(Now(),""yyyy-MM-ddT00:00:00"")+']'))
                ,[Organization].[Organization].&[413]
                ,[Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Questionnaire].&[SalesDemo]&[2]
                )";

                var dp = new DynamicParameters();
                dp.Add("~[Measures].[O]", "Open");
                dp.Add("~[Measures].[New]", "New");
                dp.Add("~[Measures].[C]", "Closed");
                dp.Add("~[Measures].[Collection Count]", "Total");

                var result = connection.Query<AlertState>(query, dp);
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void OrdnialErrorTest()
        {
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                const string query = @"WITH 
							MEMBER DisplayScore AS
							ROUND([Measures].[Response Computation], 0)

							MEMBER CalcScore AS
							ROUND([Measures].[Response Computation], 0)

							SET EntitiesWithScores AS
							NONEMPTY([Organization].[Organization].&[407].siblings,[Measures].[Response Count])

							MEMBER [Type] AS
							IIF([Questionnaire].[Question Id].currentmember is [Questionnaire].[Question Id].[All], 'Category', 'Question')							
						   
							MEMBER SqlValue AS
							IIF([Type] = 'Category',[Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.name, [Questionnaire].[Question Id].currentmember.name)

							MEMBER ParentMdxValue AS
							IIF([Type] = 'Category', null, [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.parent.uniquename)
						   
							MEMBER ParentLabel AS
							IIF([Type] = 'Category', NULL, [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].currentmember.member_caption)                            

							MEMBER [Rank] AS
							RANK([Organization].[Organization].currentmember, EntitiesWithScores, DisplayScore)

							MEMBER MaxRank AS
							MAX(EntitiesWithScores, [Rank])

							MEMBER ComparatorScore AS
										([Organization].[Organization].&[61], DisplayScore)

							MEMBER Range1 AS
							IIF([Type] = 'Category', ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Category Threshold Low Score]), ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Threshold Low Score]))

							MEMBER Range2 AS
							IIF([Type] = 'Category', ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Category Threshold High Score]), ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Threshold High Score]))

							MEMBER Range3 AS
							IIF([Type] = 'Category', ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Category Threshold Maximum Score]), ([Organization].[Organization].currentmember.datamember,[Measures].[Organization Question Threshold Maximum Score]))

							SELECT {DisplayScore, [Rank], MaxRank, [Measures].[SqlValue], [Measures].[Response Computation], [Measures].[Response Count], ParentLabel, ParentMdxValue, Range1, Range2, Range3 ,[Measures].[ComparatorScore] } on 0,
							([Organization].[Organization].&[407], [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[oa]) on 1
							FROM [Report]
							WHERE (
								   [Questionnaire].[Response Value Type].&[code],
								   [Computation].[Computation].&[Mean], 
								   [Organization].[Organization Hierarchy Name].&[Sales Demo],
								   [Report Period].[Report Period].&[November 2012],
								   [Report Period].[Report Period Type].&[3 Month Roll]
								  )";
                var parameters = new DynamicParameters();
                parameters.Add("~1", "Label");
				parameters.Add("~[Measures].[DisplayScore]", "Score");
				parameters.Add("~[Measures].[Rank]", "Rank");
                parameters.Add("~[Measures].[MaxRank]", "MaxRank");
                parameters.Add("~1##UniqueName##", "MdxValue");
				parameters.Add("~[Measures].[SqlValue]", "SqlValue");
				parameters.Add("~[Measures].[Response Computation]", "RawScore");
				parameters.Add("~[Measures].[Response Count]", "Count");
				parameters.Add("~[Measures].[ParentLabel]", "ParentLabel");
				parameters.Add("~[Measures].[ParentMdxValue]", "ParentMdxValue");
				parameters.Add("~[Measures].[Range1]", "Range1");
				parameters.Add("~[Measures].[Range2]", "Range2");
				parameters.Add("~[Measures].[Range3]", "Range3");
                parameters.Add("~[Measures].[ComparatorScore]", "ComparatorScore");

                var actual = connection.Query<Metric>(query, parameters);

                Assert.IsNotNull(actual);
                Assert.AreEqual(1, actual.Count());
            }
        }

        [TestMethod]
        public void DimensionPropertyTest()
        {
            const string query =
            @"WITH 
						
MEMBER ParentSqlValue AS
[Organization].[Organization].currentmember.parent.Properties(""Organization Id"")

SELECT {ParentSqlValue} ON 0,
DESCENDANTS([Organization].[Organization].&[61], 5 , self_and_Before) dimension properties [Organization].[Organization].[Organization Id], Parent_UNique_name ON 1
FROM Report
WHERE ([Organization].[Organization Hierarchy Name].&[Sales Demo])";

            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                
                var parms = new DynamicParameters();
                parms.Add("~0", "Label");
                parms.Add("~0##UniqueName##", "MdxValue");
                parms.Add("~0##LevelNumber##", "HierarchyLevel");
                parms.Add("~1", "SqlValue");
                parms.Add("~2", "ParentMdxValue");
                parms.Add("~3", "ParentSqlValue");

                var x = connection.Query(query, parms);

                var actual = connection.Query<Metric>(query, parms).ToList();
                Assert.IsNotNull(actual);
                Assert.AreEqual(37, actual.Count);
                var specificItem = actual.Single(a => string.Equals(a.Label, "Chicago (IL)", StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual("R11", specificItem.ParentSqlValue);
                Assert.AreEqual("U401", specificItem.SqlValue);
                Assert.AreEqual(4, specificItem.HierarchyLevel);
                Assert.AreEqual("[Organization].[Organization].&[407]", specificItem.MdxValue);
                Assert.AreEqual("[Organization].[Organization].&[61]", specificItem.ParentMdxValue);
            }
        }

        [TestMethod]
        public void DimensionPropertyWithTwoHierarchiesHavingTheSamePropertyTest()
        {
            const string query = @"WITH 

MEMBER UnitParentValue AS
[Organization].[Organization].currentmember.parent.uniquename

MEMBER DisplayScore AS
ROUND([Measures].[Response Computation], 0)
			
SELECT {[Measures].[Response Computation], DisplayScore ,[Measures].[Response Count] } ON 0,
(DESCENDANTS([Organization].[Organization].&[61],1,SELF), [Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[oa] )
DIMENSION PROPERTIES Parent_Unique_Name		on 1
FROM [Report]
WHERE 
(
	[Questionnaire].[Response Value Type].&[code],
	[Computation].[Computation].&[Mean],
	[Report Period].[Report Period].&[November 2012],
	[Organization].[Organization Hierarchy Name].&[Sales Demo],
	[Report Period].[Report Period Type].&[3 Month Roll]
)";
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();
                var parms = new DynamicParameters();

                parms.Add("~0", "SupplementalLabel");
                parms.Add("~0##UniqueName##", "SupplementalMdxValue");
                parms.Add("~0##LevelNumber##", "HierarchyLevel");
                parms.Add("~1", "Label");
                parms.Add("~1##UniqueName##", "MdxValue");
                parms.Add("~2", "SupplementalParentMdxValue");
                parms.Add("~[Measures].[DisplayScore]", "Score");
                parms.Add("~[Measures].[Response Computation]", "RawScore");
                parms.Add("~[Measures].[Response Count]", "Count");
                parms.Add("~[Measures].[ComparatorScore]", "ComparatorScore");

                var actual = connection.Query<Metric>(query, parms).ToList();

                Assert.AreEqual(36, actual.Count, "item count");
                var specificItem = actual.Single(a => a.SupplementalMdxValue == "[Organization].[Organization].&[407]");
                Assert.AreEqual(192, specificItem.Count, "return count");
                Assert.AreEqual(56, specificItem.Score, "score");
                Assert.AreEqual("Overall satisfaction", specificItem.Label, "question label");
                Assert.AreEqual("[Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[oa]", specificItem.MdxValue, "question mdxvalue");
                Assert.AreEqual(4, specificItem.HierarchyLevel, "hierarchy level");
                Assert.AreEqual("Chicago (IL)", specificItem.SupplementalLabel, "entity label");
                Assert.AreEqual("[Organization].[Organization].&[61]", specificItem.SupplementalParentMdxValue, "entity parent mdx value");
                Assert.AreEqual("56.1197916666667", specificItem.RawScore.ToString(), "raw score");

            }

        }

        [TestMethod]
        public void SpecialColumnsNoRowsTest()
        {
            const string query = @" WITH 
                            
                            MEMBER ParentMdxValue AS
							[Verbatim Taxonomy].[Verbatim Taxonomy Level 1 - 2 - 3].currentmember.parent.uniquename

							MEMBER Score AS
							[Measures].[Verbatim Sentence Sentiment Score] / [Measures].[Verbatim Analysis Sentence Count]

							MEMBER ComparatorScore AS
							CASE 
							 WHEN [Verbatim Sentence Sentiment].[Verbatim Sentence Sentiment].currentmember IS [Verbatim Sentence Sentiment].[Verbatim Sentence Sentiment].&[Negative] THEN 1.000
                             WHEN [Verbatim Sentence Sentiment].[Verbatim Sentence Sentiment].currentmember IS [Verbatim Sentence Sentiment].[Verbatim Sentence Sentiment].&[Positive] THEN 0.001
							 ELSE Score
							END

							SELECT {[Measures].[Verbatim Analysis Sentence Count], Score, ComparatorScore, ParentMdxValue} ON 0,
							NONEMPTY(DESCENDANTS([Verbatim Taxonomy].[Verbatim Taxonomy Level 1 - 2 - 3].[All], 1)*[Verbatim Sentence Sentiment].[Verbatim Sentence Sentiment].[Verbatim Sentence Sentiment],[Measures].[Verbatim Analysis Sentence Count]) ON 1
							FROM REPORT
							WHERE
							(
								[Report Period].[Report Period].&[November 2012],
								[Report Period].[Report Period Type].&[3 Month Roll],
								[Organization].[Organization Hierarchy Name].&[Sales Demo],
								[Organization].[Organization].&[407],
								[Questionnaire].[Questionnaire Version - Questionnaire - Question Category - Question].[Question].&[SalesDemo]&[2]&[oa]
								,[Population].[Population Attribute 1].[All],[Population].[Population Attribute 2].[All],[Population].[Population Attribute 3].[All]
							)";
            using (var connection = UnitTestHelpers.GetCapellaDataTestConnection())
            {
                connection.Open();

                var parms = new DynamicParameters();

                parms.Add("~[Measures].[Verbatim Analysis Sentence Count]", "Count");
				parms.Add("~[Measures].[Score]", "Score");
				parms.Add("~[Measures].[ComparatorScore]", "ComparatorScore");
				parms.Add("~0", "Label");
				parms.Add("~0##UniqueName##", "MdxValue");
                parms.Add("~0##LevelNumber##", "HierarchyLevel");
                parms.Add("~1", "SupplementalLabel");
				parms.Add("~1##UniqueName##", "SupplementalMdxValue");
                parms.Add("~[Measures].[ParentMdxValue]", "ParentMdxValue");

                var actual = connection.Query(query, parms);

                Assert.AreEqual(0, actual.Count());
            }
        }



    }
}
