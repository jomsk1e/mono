// DataColumnTest.cs - NUnit Test Cases for System.Data.DataColumn
//
// Author:
//   Franklin Wise <gracenote@earthlink.net>
//   Rodrigo Moya <rodrigo@ximian.com>
//   Daniel Morgan <danmorg@sc.rr.com>
//
// (C) Copyright 2002 Franklin Wise
// (C) Copyright 2002 Rodrigo Moya
// (C) Copyright 2003 Daniel Morgan
//

using NUnit.Framework;
using System;
using System.Data;

namespace MonoTests.System.Data
{
	public class DataColumnTest : TestCase
	{
		public DataColumnTest () : base ("System.Data.DataColumn") {}
		public DataColumnTest (string name) : base (name) {}

		private DataTable _tbl;

		protected override void SetUp () 
		{
			_tbl = new DataTable();
		}

		protected override void TearDown() {}

		public static ITest Suite {
			get { 
				return new TestSuite (typeof (DataColumnTest)); 
			}
		}

		public void TestCtor()	
		{
			string colName = "ColName";
			DataColumn col = new DataColumn();
			
			//These should all ctor without an exception
			col = new DataColumn(colName);
			col = new DataColumn(colName,typeof(int));
			col = new DataColumn(colName,typeof(int),null);
			col = new DataColumn(colName,typeof(int),null,MappingType.Attribute);

			//DataType Null
			try
			{
				col = new DataColumn(colName, null);
				Assertion.Fail("DC7: Failed to throw ArgumentNullException.");
			}
			catch (ArgumentNullException){}
			catch (AssertionFailedError exc) {throw  exc;}
			catch (Exception exc)
			{
				Assertion.Fail("DC8: DataColumnNull. Wrong exception type. Got:" + exc);
			}

		}

		public void TestAllowDBNull()
		{
			DataColumn col = new DataColumn("NullCheck",typeof(int));
			_tbl.Columns.Add(col);
			col.AllowDBNull = true;
			_tbl.Rows.Add(_tbl.NewRow());
			_tbl.Rows[0]["NullCheck"] = DBNull.Value;
			col.AllowDBNull = false;
		}

		public void TestAutoIncrement()
		{
			DataColumn col = new DataColumn("Auto",typeof(string));
			col.AutoIncrement = true;
			
			//Check for Correct Default Values
			Assertion.AssertEquals("DC9: Seed default", (long)0, col.AutoIncrementSeed);
			Assertion.AssertEquals("DC10: Step default", (long)1, col.AutoIncrementStep);

			//Check for auto type convert
			Assertion.Assert("DC11: AutoInc type convert failed." ,col.DataType == typeof (int));
		}

		public void TestAutoIncrementExceptions()
		{
			DataColumn col = new DataColumn();

			col.Expression = "SomeExpression";

			//if computed column exception is thrown
			try 
			{
				col.AutoIncrement = true;
				Assertion.Fail("DC12: Failed to throw ArgumentException");
			}
			catch (ArgumentException){}
			catch (AssertionFailedError exc) {throw  exc;}
			catch (Exception exc)
			{
				Assertion.Fail("DC13: ExprAutoInc. Wrong exception type. Got:" + exc);
			}


		}

		public void TestCaption()
		{
			DataColumn col = new DataColumn("ColName");
			//Caption not set at this point
			Assertion.AssertEquals("DC14: Caption Should Equal Col Name", col.ColumnName, col.Caption);

			//Set caption
			col.Caption = "MyCaption";
			Assertion.AssertEquals("DC15: Caption should equal caption.", "MyCaption", col.Caption);

			//Clear caption
			col.Caption = null;
			Assertion.AssertEquals("DC16: Caption Should Equal Col Name after clear", col.ColumnName, col.Caption);
			
		}

		public void TestForColumnNameException()
		{
			DataColumn col = new DataColumn();
			DataColumn col2 = new DataColumn();
			DataColumn col3 = new DataColumn();
			DataColumn col4 = new DataColumn();
			
			col.ColumnName = "abc";
			AssertEquals( "abc", col.ColumnName);

			_tbl.Columns.Add(col);
			
			//Duplicate name exception
			try
			{
				col2.ColumnName = "abc";
				_tbl.Columns.Add(col2);
				AssertEquals( "abc", col2.ColumnName);
				Assertion.Fail("DC17: Failed to throw duplicate name exception.");
			}
			catch (DuplicateNameException){}
			catch (AssertionFailedError exc) {throw  exc;}
			catch (Exception exc)
			{
				Assertion.Fail("DC18: Wrong exception type. " + exc.ToString());
			}

			// Make sure case matters in duplicate checks
			col3.ColumnName = "ABC";
			_tbl.Columns.Add(col3);
		}

		public void TestDefaultValue()
		{
			DataTable tbl = new DataTable();
			tbl.Columns.Add("MyCol", typeof(int));
			
			//Set default Value if Autoincrement is true
			tbl.Columns[0].AutoIncrement = true;
			try
			{
				tbl.Columns[0].DefaultValue = 2;
				Assertion.Fail("DC19: Failed to throw ArgumentException.");
			}
			catch (ArgumentException){}
			catch (AssertionFailedError exc) {throw  exc;}
			catch (Exception exc)
			{
				Assertion.Fail("DC20: Wrong exception type. " + exc.ToString());
			}


			tbl.Columns[0].AutoIncrement = false;

			//Set default value to an incompatible datatype
			try
			{
				tbl.Columns[0].DefaultValue = "hello";
				Assertion.Fail("DC21: Failed to throw InvalidCastException.");
			}
			catch (InvalidCastException){}
			catch (AssertionFailedError exc) {throw  exc;}
			catch (Exception exc)
			{
				Assertion.Fail("DC22: Wrong exception type. " + exc.ToString());
			}

			//TODO: maybe add tests for setting default value for types that can implict
			//cast




		}

		public void TestSetDataType()
		{
			//test for DataAlready exists and change the datatype
			
			//supported datatype

			//AutoInc column dataType supported

		}

		public void TestDefaults1() 
		{
			//Check for defaults - ColumnName not set at the beginning
			DataTable table = new DataTable();		
			DataColumn column = new DataColumn();
			
			Assertion.AssertEquals("DC1: ColumnName default Before Add", column.ColumnName, String.Empty);
			Assertion.AssertEquals("DC2: DataType default Before Add", column.DataType.ToString(), typeof(string).ToString());
			
			table.Columns.Add(column);
			
			Assertion.AssertEquals("DC3: ColumnName default After Add", table.Columns[0].ColumnName, "Column1");
			Assertion.AssertEquals("DC4: DataType default After Add", table.Columns[0].DataType.ToString(), typeof(string).ToString());	
			
			DataRow row = table.NewRow();
			table.Rows.Add(row);
			DataRow dataRow = table.Rows[0];
			
			object v = null;
			try {
				v = dataRow.ItemArray[0];
			}
			catch(Exception e) {
				Assertion.Fail("DC5: getting item from dataRow.ItemArray[0] threw Exception: " + e);
			}
			
			Type vType = dataRow.ItemArray[0].GetType();
			Assertion.AssertEquals("DC6: Value from DataRow.Item", v, DBNull.Value);
		}

		public void TestDefaults2() 
		{
			//Check for defaults - ColumnName set at the beginning
			string blah = "Blah";
			//Check for defaults - ColumnName not set at the beginning
			DataTable table = new DataTable();		
			DataColumn column = new DataColumn(blah);
			
			Assertion.AssertEquals("DC23: ColumnName default Before Add", column.ColumnName,blah);
			Assertion.AssertEquals("DC24: DataType default Before Add", column.DataType.ToString(), typeof(string).ToString());
			
			table.Columns.Add(column);
			
			Assertion.AssertEquals("DC25: ColumnName default After Add", table.Columns[0].ColumnName, blah);
			Assertion.AssertEquals("DC26: DataType default After Add", table.Columns[0].DataType.ToString(), typeof(string).ToString());	
			
			DataRow row = table.NewRow();
			table.Rows.Add(row);
			DataRow dataRow = table.Rows[0];

			object v = null;
			try {
				v = dataRow.ItemArray[0];
			}
			catch(Exception e) {
				Assertion.Fail("DC27: getting item from dataRow.ItemArray[0] threw Exception: " + e);
			}
			
			Type vType = dataRow.ItemArray[0].GetType();
			Assertion.AssertEquals("DC28: Value from DataRow.Item", v, DBNull.Value);
		}

                public void TestExpressionFunctions ()
                {
                	DataTable T = new DataTable ("test");
			DataColumn C = new DataColumn ("name");
			T.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T.Columns.Add (C);
			C = new DataColumn ("id");
                	C.Expression = "substring (name, 1, 3) + len (name) + age";
			T.Columns.Add (C);
			
			DataSet Set = new DataSet ("TestSet");
			Set.Tables.Add (T);
			
			DataRow Row = null;
			for (int i = 0; i < 100; i++) {
				Row = T.NewRow ();
				Row [0] = "human" + i;
				Row [1] = i;
				T.Rows.Add (Row);
			}
			
			Row = T.NewRow ();
			Row [0] = "h*an";
			Row [1] = DBNull.Value;
			T.Rows.Add (Row);
						
			AssertEquals ("DC29", "hum710", T.Rows [10] [2]);
			AssertEquals ("DC30", "hum64", T.Rows [4] [2]);
                	C = T.Columns [2];
                	C.Expression = "isnull (age, 'succ[[]]ess')";
                	AssertEquals ("DC31", "succ[[]]ess", T.Rows [100] [2]);
                	
                	C.Expression = "iif (age = 24, 'hurrey', 'boo')";
                	AssertEquals ("DC32", "boo", T.Rows [50] [2]);
	               	AssertEquals ("DC33", "hurrey", T.Rows [24] [2]);
                	
                	C.Expression = "convert (age, 'System.Boolean')";
                	AssertEquals ("DC32", Boolean.TrueString, T.Rows [50] [2]);
                	AssertEquals ("DC32", Boolean.FalseString, T.Rows [0] [2]);
                	
                	//
                	// Exceptions
                	//
                	
                	try {
                		C.Expression = "iff (age = 24, 'hurrey', 'boo')";
                		Fail ("DC34");
                	} catch (Exception e) {
                		                	
                		// The expression contains undefined function call iff().
                		AssertEquals ("DC35", typeof (EvaluateException), e.GetType ());
                	}
                	
                	try {
                		C.Expression = "iif (nimi = 24, 'hurrey', 'boo')";
                		Fail ("DC36");
                	} catch (Exception e) {                		               	
                		AssertEquals ("DC37", typeof (EvaluateException), e.GetType ());
                		AssertEquals ("DC38", "Cannot find column [nimi].", e.Message);
                	}
                	
                	try {
                		C.Expression = "iif (name = 24, 'hurrey', 'boo')";
                		Fail ("DC39");
                	} catch (Exception e) {
                		AssertEquals ("DC40", typeof (EvaluateException), e.GetType ());
                		AssertEquals ("DC41", "Cannot perform '=' operation on System.String and System.Int32.", e.Message);
                	}
                	

                	try {
                		C.Expression = "convert (age, Boolean)";	
                		Fail ("DC42");
                	} catch (Exception e) {
                		AssertEquals ("DC43", typeof (EvaluateException), e.GetType ());
                		AssertEquals ("DC44", "Invalid type name 'Boolean'.", e.Message);
                	}
                	
                }

                public void TestExpressionAggregates ()
                {
                	DataTable T = new DataTable ("test");
			DataTable T2 = new DataTable ("test2");
			
			DataColumn C = new DataColumn ("name");
			T.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T.Columns.Add (C);
			C = new DataColumn ("childname");
			T.Columns.Add (C);
                	
			C = new DataColumn ("expression");
			T.Columns.Add (C);

			DataSet Set = new DataSet ("TestSet");
			Set.Tables.Add (T);
			Set.Tables.Add (T2);
			
			DataRow Row = null;
			for (int i = 0; i < 100; i++) {
				Row = T.NewRow ();
				Row [0] = "human" + i;
				Row [1] = i;
				Row [2] = "child" + i;
				T.Rows.Add (Row);
			}
			
			Row = T.NewRow ();
			Row [0] = "h*an";
			Row [1] = DBNull.Value;
			T.Rows.Add (Row);

			C = new DataColumn ("name");
                	T2.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T2.Columns.Add (C);
                	
			for (int i = 0; i < 100; i++) {
				Row = T2.NewRow ();
				Row [0] = "child" + i;
				Row [1] = i;
				T2.Rows.Add (Row);
				Row = T2.NewRow ();
				Row [0] = "child" + i;
				Row [1] = i - 2;
				T2.Rows.Add (Row);
			}
                	
                	DataRelation Rel = new DataRelation ("Rel", T.Columns [2], T2.Columns [0]);
                	Set.Relations.Add (Rel);
                	
                	C = T.Columns [3];
                	C.Expression = "Sum (Child.age)";
                	AssertEquals ("DC45", "-2", T.Rows [0] [3]);
                	AssertEquals ("DC46", "98", T.Rows [50] [3]);
                	
			C.Expression = "Count (Child.age)";
                	AssertEquals ("DC47", "2", T.Rows [0] [3]);
                	AssertEquals ("DC48", "2", T.Rows [60] [3]);		                	
		
			C.Expression = "Avg (Child.age)";
                	AssertEquals ("DC49", "-1", T.Rows [0] [3]);
                	AssertEquals ("DC50", "59", T.Rows [60] [3]);		                	

			C.Expression = "Min (Child.age)";
                	AssertEquals ("DC51", "-2", T.Rows [0] [3]);
                	AssertEquals ("DC52", "58", T.Rows [60] [3]);		                	

			C.Expression = "Max (Child.age)";
                	AssertEquals ("DC53", "0", T.Rows [0] [3]);
                	AssertEquals ("DC54", "60", T.Rows [60] [3]);		                	

			C.Expression = "stdev (Child.age)";
                	AssertEquals ("DC55", "1,4142135623731", T.Rows [0] [3]);
                	AssertEquals ("DC56", "1,4142135623731", T.Rows [60] [3]);		                	

			C.Expression = "var (Child.age)";
                	AssertEquals ("DC57", "2", T.Rows [0] [3]);
                	AssertEquals ("DC58", "2", T.Rows [60] [3]);		                	
                }

		public void TestExpressionOperator ()
		{
                	DataTable T = new DataTable ("test");
			DataColumn C = new DataColumn ("name");
			T.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T.Columns.Add (C);
			C = new DataColumn ("id");
                	C.Expression = "substring (name, 1, 3) + len (name) + age";
			T.Columns.Add (C);
			
			DataSet Set = new DataSet ("TestSet");
			Set.Tables.Add (T);
			
			DataRow Row = null;
			for (int i = 0; i < 100; i++) {
				Row = T.NewRow ();
				Row [0] = "human" + i;
				Row [1] = i;
				T.Rows.Add (Row);
			}
			
			Row = T.NewRow ();
			Row [0] = "h*an";
			Row [1] = DBNull.Value;
			T.Rows.Add (Row);
			
                	C = T.Columns [2];
                	C.Expression = "age + 4";
			AssertEquals ("DC59", "68", T.Rows [64] [2]);
			
			C.Expression = "age - 4";
			AssertEquals ("DC60", "60", T.Rows [64] [2]);
			
			C.Expression = "age * 4";
			AssertEquals ("DC61", "256", T.Rows [64] [2]);
			
			C.Expression = "age / 4";
			AssertEquals ("DC62", "16", T.Rows [64] [2]);
			
			C.Expression = "age % 5";
			AssertEquals ("DC63", "4", T.Rows [64] [2]);
			
			C.Expression = "age in (5, 10, 15, 20, 25)";
			AssertEquals ("DC64", "False", T.Rows [64] [2]);
			AssertEquals ("DC65", "True", T.Rows [25] [2]);
			
			C.Expression = "name like 'human1%'";
			AssertEquals ("DC66", "True", T.Rows [1] [2]);
			AssertEquals ("DC67", "False", T.Rows [25] [2]);

                	C.Expression = "age < 4";
			AssertEquals ("DC68", "False", T.Rows [4] [2]);
			AssertEquals ("DC69", "True", T.Rows [3] [2]);

                	C.Expression = "age <= 4";
			AssertEquals ("DC70", "True", T.Rows [4] [2]);
			AssertEquals ("DC71", "False", T.Rows [5] [2]);

                	C.Expression = "age > 4";
			AssertEquals ("DC72", "False", T.Rows [4] [2]);
			AssertEquals ("DC73", "True", T.Rows [5] [2]);

                	C.Expression = "age >= 4";
			AssertEquals ("DC74", "True", T.Rows [4] [2]);
			AssertEquals ("DC75", "False", T.Rows [1] [2]);

                	C.Expression = "age = 4";
			AssertEquals ("DC76", "True", T.Rows [4] [2]);
			AssertEquals ("DC77", "False", T.Rows [1] [2]);

                	C.Expression = "age <> 4";
			AssertEquals ("DC76", "False", T.Rows [4] [2]);
			AssertEquals ("DC77", "True", T.Rows [1] [2]);
		}
	}
}
