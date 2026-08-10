using System;
using System.Data.SqlClient;

class Program {
    static void Main() {
        string connStr = ""Server=db62212.databaseasp.net; Database=db62212; User Id=db62212; Password=j_8T6rY-!4iD; Encrypt=False; MultipleActiveResultSets=True;"";
        using (var conn = new SqlConnection(connStr)) {
            conn.Open();
            // Get Esraa's ApplicationUserId
            string getUserId = ""SELECT u.Id, u.Email FROM AspNetUsers u JOIN Employees e ON u.Id = e.ApplicationUserId WHERE e.FullName LIKE N'%ÇÓÑÇÁ%'"";
            string userId = null;
            string email = null;
            using (var cmd = new SqlCommand(getUserId, conn)) {
                using (var reader = cmd.ExecuteReader()) {
                    if (reader.Read()) {
                        userId = reader[""Id""].ToString();
                        email = reader[""Email""].ToString();
                        Console.WriteLine($""Esraa UserId: {userId}, Email: {email}"");
                    }
                }
            }
            if (userId != null) {
                string getClaims = $""SELECT ClaimType, ClaimValue FROM AspNetUserClaims WHERE UserId = '{userId}'"";
                using (var cmd = new SqlCommand(getClaims, conn)) {
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            Console.WriteLine($""Claim: {reader[""ClaimType""]} = {reader[""ClaimValue""]}"");
                        }
                    }
                }
            }
        }
    }
}
