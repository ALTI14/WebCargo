using Microsoft.Data.Sqlite;

namespace WebCargo.Models
{
    public static class Database
    {
        public static IConfiguration? Configuration
        {
            get;
            set;
        }

        private static string ConnectionString => 
            Configuration!["ConnectionStrings:DefaultConnection"]!;
        public static void CreateDatabase()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Kargolar
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER,
                TrackingNumber TEXT UNIQUE,
                Alici TEXT,
                Tc TEXT,
                Adres TEXT,
                Email TEXT,
                Durum TEXT,
                Tarih TEXT
            );
            ";

            command.ExecuteNonQuery();

            command.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Users
            (
             Id INTEGER PRIMARY KEY AUTOINCREMENT,
             Username TEXT,
             PasswordHash TEXT,
             Email TEXT,
             Tc TEXT,
             RegisterDate TEXT
             
            );
            ";

            command.ExecuteNonQuery();
        }

        public static void CreateInvitedUser(
    string email)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            INSERT INTO Users
            (Email)
            VALUES
            ($email)
            ";

            command.Parameters.AddWithValue(
                "$email",
                email
            );

            command.ExecuteNonQuery();
        }

        public static void CompleteRegistration(
    string username,
    string passwordHash,
    string email,
    string tc,
    DateTime registerDate)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            UPDATE Users
            SET
            Username = $username,
            PasswordHash = $passwordHash,
            Tc = $tc,
            RegisterDate = $registerDate
            WHERE Email = $email
            ";

            command.Parameters.AddWithValue(
                "$username",
                username
            );

            command.Parameters.AddWithValue(
                "$passwordHash",
                passwordHash
            );

            command.Parameters.AddWithValue(
                "$tc",
                tc
            );

            command.Parameters.AddWithValue(
                "$registerDate",
                registerDate
            );

            command.Parameters.AddWithValue(
                "$email",
                email
            );

            command.ExecuteNonQuery();
        }


        public static bool UserExists(
    string username)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            SELECT COUNT(*)
            FROM Users
            WHERE Username = $username
            ";

            command.Parameters.AddWithValue(
                "$username",
                username
            );

            long count =
                (long)command.ExecuteScalar()!;

            return count > 0;
        }


        public static long? GetId(
    string username)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            SELECT Id
            FROM Users
            WHERE Username = $username
            ";

            command.Parameters.AddWithValue(
                "$username",
                username
            );

            return (long?)
                command.ExecuteScalar();
        }

        public static bool Login(
    string username,
    string passwordHash)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            SELECT COUNT(*)
            FROM Users
            WHERE Username = $username
            AND PasswordHash = $passwordHash
            ";

            command.Parameters.AddWithValue(
                "$username",
                username
            );

            command.Parameters.AddWithValue(
                "$passwordHash",
                passwordHash
            );

            long count =
                (long)command.ExecuteScalar()!;

            return count > 0;
        }


        public static List<Cargo> GetAllCargo() // kargoları yüklemek için
        {
            List<Cargo> kargolar = new();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
                @"
                SELECT * FROM Kargolar
                ";
            var reader =
            command.ExecuteReader();

            while (reader.Read())
            {
                Cargo cargo = new Cargo();

                cargo.Id =
                    reader.GetInt32(0);

                cargo.UserId =
                    reader.GetInt32(1);

                cargo.TrackingNumber =
                    reader.GetString(2);

                cargo.Alici =
                    reader.GetString(3);

                cargo.Tc = 
                    reader.GetString(4);

                cargo.Adres = 
                    reader.GetString(5);

                cargo.Email =
                    reader.GetString(6);

                cargo.Durum =
                    reader.GetString(7);

                cargo.SonDurumDegisiklikTarihi =
                    DateTime.Parse(
                        reader.GetString(8)
                    );

                kargolar.Add(cargo);
            }

            return kargolar;
        }

        public static User? GetUserByEmail(
    string email)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            SELECT *
            FROM Users
            WHERE Email = $email
             ";

            command.Parameters.AddWithValue(
                "$email",
                email
            );

            using var reader =
                command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            User user =
                new();

            user.Id =
                Convert.ToInt32(
                    reader["Id"]
                );

            user.Username =
                reader["Username"]
                .ToString() ?? "";

            user.PasswordHash =
                reader["PasswordHash"]
                .ToString() ?? "";

            user.Email =
                reader["Email"]
                .ToString() ?? "";

            user.Tc =
                reader["Tc"]
                .ToString() ?? "";

            string? registerDate =
                reader["RegisterDate"]
                .ToString();

            if (!string.IsNullOrWhiteSpace(
                registerDate))
            {
                user.RegisterDate =
                    DateTime.Parse(
                        registerDate
                    );
            }

            return user;
        }

        public static void UpdatePassword(
    string email,
    string passwordHash)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            UPDATE Users
            SET PasswordHash = $passwordHash
            WHERE Email = $email
            ";

            command.Parameters.AddWithValue(
                "$passwordHash",
                passwordHash
            );

            command.Parameters.AddWithValue(
                "$email",
                email
            );

            command.ExecuteNonQuery();
        }

        public static void InsertCargo(Cargo cargo) // kargo eklemek için
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
                @" 
                INSERT INTO Kargolar
                (UserId, TrackingNumber, Alici, Tc, Adres, Email, Durum, Tarih)
                VALUES
                ($userId, $trackingNumber, $alici, $tc, $adres, $email, $durum, $tarih)
                ";
            command.Parameters.AddWithValue(
                "$userId",
                cargo.UserId
                );
            command.Parameters.AddWithValue(
                "$trackingNumber",
                cargo.TrackingNumber
                );
            command.Parameters.AddWithValue(
                "$alici",
                cargo.Alici);
            command.Parameters.AddWithValue(
                "$tc",
                cargo.Tc);
            command.Parameters.AddWithValue(
                "$adres",
                cargo.Adres);
            command.Parameters.AddWithValue(
                "$email",
                cargo.Email);
            command.Parameters.AddWithValue(
                "$durum",
                cargo.Durum);
            command.Parameters.AddWithValue(
                "$tarih",
                cargo.SonDurumDegisiklikTarihi);
            command.ExecuteNonQuery();

            command.CommandText = // az once eklenen kaydın Id'si
            "SELECT last_insert_rowid();";

            cargo.Id =
                Convert.ToInt32(
                    command.ExecuteScalar()
                );
        }
        public static void UpdateCargo(
     Cargo cargo)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            UPDATE Kargolar
            SET
            Alici = $alici,
            Tc = $tc,
            Adres = $adres,
            Email = $email,
            Durum = $durum,
            Tarih = $tarih
            WHERE
            Id = $id
            ";

            command.Parameters.AddWithValue(
                "$alici",
                cargo.Alici
            );

            command.Parameters.AddWithValue(
                "$tc",
                cargo.Tc
            );

            command.Parameters.AddWithValue(
                "$adres",
                cargo.Adres
            );

            command.Parameters.AddWithValue(
                "$email",
                cargo.Email
            );

            command.Parameters.AddWithValue(
                "$durum",
                cargo.Durum
            );

            command.Parameters.AddWithValue(
                "$tarih",
                cargo.SonDurumDegisiklikTarihi
            );

            command.Parameters.AddWithValue(
                "$id",
                cargo.Id
            );

            command.ExecuteNonQuery();
        }

        public static string? GetEmailById( // şu an kullanılmıyor
    int id)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            SELECT Email
            FROM Users
            WHERE Id = $id
            ";

            command.Parameters.AddWithValue(
                "$id",
                id
            );

            return (string?)
                command.ExecuteScalar();
        }

        public static void DeleteCargo(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();


            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            DELETE FROM Kargolar
            WHERE Id = $id
            ";

            command.Parameters.AddWithValue(
                "$id",
                id
            );

            command.ExecuteNonQuery();
        }

        public static Cargo? GetCargoByTrackingNumber(
    string trackingNumber)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            SELECT *
            FROM Kargolar
            WHERE TrackingNumber =
            $trackingNumber
            ";

            command.Parameters.AddWithValue(
                "$trackingNumber",
                trackingNumber
            );

            using var reader =
                command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            Cargo cargo = new();

            cargo.Id =
                reader.GetInt32(0);

            cargo.UserId =
                reader.GetInt32(1);

            cargo.TrackingNumber =
                reader.GetString(2);

            cargo.Alici =
                reader.GetString(3);

            cargo.Tc =
                reader.GetString(4);

            cargo.Adres =
                reader.GetString(5);

            cargo.Email =
                reader.GetString(6);

            cargo.Durum =
                reader.GetString(7);

            cargo.SonDurumDegisiklikTarihi =
                DateTime.Parse(
                    reader.GetString(8)
                );

            return cargo;
        }

        public static List<User> GetUserPage(
    int page,
    int pageSize)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            SELECT *
            FROM Users
            LIMIT $pageSize
            OFFSET $offset
            ";

            command.Parameters.AddWithValue(
                "$pageSize",
                pageSize + 1
            );

            command.Parameters.AddWithValue(
                "$offset",
                (page - 1) * pageSize
            );

            List<User> users =
                new();

            using var reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                User user =
                    new();

                user.Id =
                    reader.GetInt32(0);

                user.Username =
                    reader.IsDBNull(1)
                    ? ""
                    : reader.GetString(1);

                user.PasswordHash =
                    reader.IsDBNull(2)
                    ? ""
                    : reader.GetString(2);

                user.Email =
                    reader.IsDBNull(3)
                    ? ""
                    : reader.GetString(3);

                user.Tc =
                    reader.IsDBNull(4)
                    ? ""
                    : reader.GetString(4);

                if (!reader.IsDBNull(5))
                {
                    user.RegisterDate =
                        DateTime.Parse(
                            reader.GetString(5)
                        );
                }

                users.Add(user);
            }

            return users;
        }

        public static void DeleteUser(
    int id)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            DELETE FROM Users
            WHERE Id = $id
            ";

            command.Parameters.AddWithValue(
                "$id",
                id
            );

            command.ExecuteNonQuery();
        }

        public static Cargo? GetCargoById(
    int id)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            SELECT *
            FROM Kargolar
            WHERE Id = $id
            ";

            command.Parameters.AddWithValue(
                "$id",
                id
            );

            using var reader =
                command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            Cargo cargo =
                new();

            cargo.Id =
                reader.GetInt32(0);

            cargo.UserId =
                reader.GetInt32(1);

            cargo.TrackingNumber =
                reader.GetString(2);

            cargo.Alici =
                reader.GetString(3);

            cargo.Tc =
                reader.GetString(4);

            cargo.Adres =
                reader.GetString(5);

            cargo.Email =
                reader.GetString(6);

            cargo.Durum =
                reader.GetString(7);

            cargo.SonDurumDegisiklikTarihi =
                DateTime.Parse(
                    reader.GetString(8)
                );

            return cargo;
        }

        public static bool TrackingNumberExists(
    string trackingNumber)
        {
            // SQLite bağlantısı oluştur
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );


            connection.Open(); // veritabanini ac

            // SQL komutu oluştur
            using var command =
                connection.CreateCommand();

            // Takip numarasına sahip kaç kayıt var bul
            command.CommandText =
                """
                SELECT COUNT(*)
                FROM Kargolar
                WHERE TrackingNumber =
                $trackingNumber
                """;

            command.Parameters.AddWithValue(
                "$trackingNumber",
                trackingNumber
            );

            long count =
                (long)command.ExecuteScalar()!;

            return count > 0;
        }

        public static List<Cargo> GetCargoPage(
    int page,
    int pageSize)
        {
            using var connection =
                new SqliteConnection(
                    ConnectionString
                );

            connection.Open();

            var command =
                connection.CreateCommand();

            command.CommandText =
            @"
            SELECT *
            FROM Kargolar
            LIMIT $pageSize
            OFFSET $offset
            ";

            // Her sayfada gösterilecek kayıt sayısı + 1
            command.Parameters.AddWithValue(
                "$pageSize",
                pageSize + 1
            );

            // Atlanacak kayıt sayısı
            command.Parameters.AddWithValue(
                "$offset",
                (page - 1) * pageSize
            );

            List<Cargo> kargolar =
                new();

            using var reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                Cargo cargo =
                    new();

                cargo.Id =
                    reader.GetInt32(0);

                cargo.UserId =
                    reader.GetInt32(1);

                cargo.TrackingNumber =
                    reader.GetString(2);

                cargo.Alici =
                    reader.GetString(3);

                cargo.Tc =
                    reader.GetString(4);

                cargo.Adres =
                    reader.GetString(5);

                cargo.Email =
                    reader.GetString(6);

                cargo.Durum =
                    reader.GetString(7);

                cargo.SonDurumDegisiklikTarihi =
                    DateTime.Parse(
                        reader.GetString(8)
                    );

                kargolar.Add(
                    cargo
                );
            }

            return kargolar;
        }


    }
}