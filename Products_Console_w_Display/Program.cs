// See https://aka.ms/new-console-template for more information


using System;
using System.Data;
using Npgsql;


class Sample
{
    static void Main(string[] args)
    {
       
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1:5432;User Id=postgres; " +
           "Password=Khobe@0825;Database=prods;");
        conn.Open();

       
        NpgsqlCommand command_q = new NpgsqlCommand("SELECT * FROM product", conn);

        // Execute the query and obtain the value of the first column of the first row
        //Int64 count = (Int64)command.ExecuteScalar();
        NpgsqlDataReader reader_q = command_q.ExecuteReader();

        DataTable dt_q = new DataTable();
        dt_q.Load(reader_q);

        print_quantity(dt_q);

        
        NpgsqlCommand command_r = new NpgsqlCommand("SELECT * FROM customer", conn);

  
        NpgsqlDataReader reader_r = command_r.ExecuteReader();

        DataTable dt_r = new DataTable();
        dt_r.Load(reader_r);

        print_rep(dt_r);

        conn.Close();
    }

    static void print_quantity(DataTable data)
    {
        Console.WriteLine();
        Dictionary<string, int> colWidths = new Dictionary<string, int>();

        foreach (DataColumn col in data.Columns)
        {
            if (col.ColumnName is "prod_id" or "prod_desc" or "prod_quantity")
            {
                Console.Write(col.ColumnName);
            }
            var maxLabelSize = data.Rows.OfType<DataRow>()
                    .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                    .OrderByDescending(m => m).FirstOrDefault();

            colWidths.Add(col.ColumnName, maxLabelSize);
            for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++)
                Console.Write(" ");
        }

        Console.WriteLine();

        foreach (DataRow dataRow in data.Rows)
        {
            for (int j = 0; j < 3; j++)
            {
                if (Convert.ToInt32(dataRow.ItemArray[2]) is <= 30 and >= 12)
                    Console.Write(dataRow.ItemArray[j]);
                else
                    ;
                for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++)
                    Console.Write(" ");
            }
            Console.WriteLine();
        }
    }


    static void print_rep(DataTable data)
    {
        Console.WriteLine();
        Dictionary<string, int> colWidths = new Dictionary<string, int>();

        List<string> colset = new List<string> { };

        foreach (DataColumn col in data.Columns)
        {
            colset.Add(col.ColumnName);
        }

        colset.Reverse();

        foreach (string col in colset)
        {
            if (col is "rep_id" or "cust_balance")
            {
                Console.Write(col + "       ");
            }

            var maxLabelSize = data.Rows.OfType<DataRow>()
                    .Select(m => (m.Field<object>(col)?.ToString() ?? "").Length)
                    .OrderByDescending(m => m).FirstOrDefault();

            colWidths.Add(col, maxLabelSize);
            
        }

        Console.WriteLine();

        Dictionary<string, decimal> reps = new Dictionary<string, decimal>();

        decimal sum;

        foreach (DataRow dataRow in data.Rows)
        {
            if (!reps.ContainsKey(dataRow["rep_id"].ToString())) 
            {
                sum = (decimal)dataRow["cust_balance"];
                reps.Add(dataRow["rep_id"].ToString(), sum);
            }
            else
            {
                sum = reps[dataRow["rep_id"].ToString()] + (decimal)dataRow["cust_balance"];
                reps[dataRow["rep_id"].ToString()] = sum;
            }
        }

        foreach (var entry in reps)
        {
            if (entry.Value is > 12000)
                Console.WriteLine(entry.Key + "          " + entry.Value);
        }
    }

}
