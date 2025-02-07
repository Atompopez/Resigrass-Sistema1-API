using Npgsql;
using ResiGrass_API.Models;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using DocumentFormat.OpenXml.Office.Word;
using System.Data;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using static IdentityServer4.Models.IdentityResources;
using iText.StyledXmlParser.Jsoup.Select;



namespace ResiGrass_API.Logic
{
    public class DbQuery
    {
        private readonly string _connectionString;

        public DbQuery(string connectionString)
        {
            _connectionString = connectionString;            
        }

        #region GetNextNumber
        public int GetNextNumber(int IdCollector)
        {
            string nextSerial = "0";
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    string serialQuery = @"
                            SELECT ""serial_number""
                            FROM collection
                            WHERE ""collectorId"" = @collectorId
                            ORDER BY ""serial_number"" DESC
                            LIMIT 1";

                    using (var serialCmd = new NpgsqlCommand(serialQuery, conn))
                    {
                        serialCmd.Parameters.AddWithValue("@collectorId", IdCollector);

                        var result = serialCmd.ExecuteScalar();
                        

                        if (result != null)
                        {
                            var lastSerial = result.ToString();
                            var lastNumber = int.Parse(lastSerial.Split('-').Last());
                            nextSerial = $"{(lastNumber + 1):D1}";
                        }
                        else
                        {
                            nextSerial = $"1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de negocio: {ex}");
            }
            return int.Parse(nextSerial);
        }
        #endregion

        #region Municipalities
        public List<MunicipalityModel> GetMunicipalities()
        {
            var municipalities = new List<MunicipalityModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("SELECT * FROM municipality WHERE CAST(status AS INTEGER) != 0;", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                                {
                                    var municipality = new MunicipalityModel
                                    {
                                        id = reader.GetInt32(0),
                                        nameCity = reader.GetString(1),
                                        status = reader.GetBoolean(2)
                                    };

                                    municipalities.Add(municipality);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener los municipios: {ex.Message}");

                return new List<MunicipalityModel>();
            }

            return municipalities;
        }
        #endregion

        #region Localities
        public List<LocalitiesModel> GetLocalities(int idMunicipality)
        {
            var localities = new List<LocalitiesModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;
                    if (idMunicipality == 0)
                    {
                        query =
                            "SELECT *  FROM locality INNER JOIN municipality on locality.\"municipalityId\" = municipality.id ";
                    }
                    else
                    {
                        query =
                            "SELECT *  FROM locality INNER JOIN municipality on locality.\"municipalityId\" = municipality.id WHERE \"municipalityId\" = @idMunicipality";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idMunicipality", idMunicipality);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                                {
                                    var locality = new LocalitiesModel
                                    {
                                        id = reader.GetInt32(0),
                                        nameLocality = reader.GetString(1),
                                        municipalityId = reader.GetInt32(3),
                                        MunicipalityData = new MunicipalityModel
                                        {
                                            id = reader.GetInt32(4),
                                            nameCity = reader.GetString(5),
                                            status = reader.GetBoolean(6),

                                        }
                                    };

                                    localities.Add(locality);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener las localidades: {ex.Message}");

                return new List<LocalitiesModel>();
            }

            return localities;
        }
        #endregion

        #region Localities
        public List<CollectorsModel> GetToken(int id)
        {
            var Collector = new List<CollectorsModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string  query = "SELECT id, \"nameCollector\", \"numberPhoneCollector\", \"dateCreationCollector\", status, \"loginCollectorId\", \"typeCollectorId\", profile_image\r\n\tFROM resigrass.collector WHERE id = @id ";


                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                                {
                                    var collector = new CollectorsModel
                                    {
                                        id = reader.GetInt32(0),
                                        nameCollector = reader.GetString(1),
                                        numberPhoneCollector = reader.GetString(2),                                       
                                        status = reader.GetBoolean(4),
                                        dateCreationCollector = reader.GetDateTime(3),

                                    };
                                    var profileImageBytes = reader.IsDBNull(7) ? null : (byte[])reader[7];

                                    collector.profile_image = profileImageBytes != null ? Convert.ToBase64String(profileImageBytes) : null;

                                    Collector.Add(collector);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener las localidades: {ex.Message}");

                return new List<CollectorsModel>();
            }

            return Collector;
        }
        #endregion 

        #region TypeBusiness
        public List<TypeBusinessModel> GetTypeBusiness()
        {
            var typebusiness = new List<TypeBusinessModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("SELECT * FROM \"typeBusiness\"", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                                {
                                    var typeBusiness = new TypeBusinessModel
                                    {
                                        id = reader.GetInt32(0),
                                        businessDescription = reader.GetString(1),
                                        status = reader.GetBoolean(2)
                                    };

                                    typebusiness.Add(typeBusiness);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener los tipos de negocio: {ex.Message}");

                return new List<TypeBusinessModel>();
            }

            return typebusiness;
        }
        #endregion

        #region Client
        public List<ClientModel> GetClients(int idTypeBusiness)
        {
            var clients = new List<ClientModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                      SELECT c.""id"", c.""nameClient"", c.""corporate_name"", c.""dateCreationClient"", 
                   c.""status"", c.""typeBusinessId"",
                   t.""id"", t.""businessDescription"", t.""status""
            FROM ""client"" c
            INNER JOIN ""typeBusiness"" t ON c.""typeBusinessId"" = t.""id""
			
        ";

                    if (idTypeBusiness == 0)
                    {
                        query += @" WHERE c.""status"" = '1'";
                    }
                    else if (idTypeBusiness == -1)
                    {
                        // No se filtra por ningún criterio adicional.
                    }
                    else
                    {
                        query += @" WHERE c.""typeBusinessId"" = @idTypeBusiness AND c.""status"" = '1'";
                    }
                    query += @"ORDER BY C.""nameClient""";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (idTypeBusiness > 0)
                        {
                            cmd.Parameters.AddWithValue("idTypeBusiness", idTypeBusiness);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var client = new ClientModel
                                {
                                    id = reader.GetInt32(0),
                                    nameClient = reader.GetString(1),
                                    corporateName = reader.IsDBNull(2) ? null : reader.GetString(2), // Manejo de valores nulos
                                    dateCreationClient = reader.GetDateTime(3),
                                    status = reader.GetBoolean(4),
                                    typeBusinessId = reader.GetInt32(5),
                                    businessModelData = new TypeBusinessModel
                                    {
                                        id = reader.GetInt32(6),
                                        businessDescription = reader.GetString(7),
                                        status = reader.GetBoolean(8)
                                    }
                                };

                                clients.Add(client);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los clientes: {ex.Message}");
                return new List<ClientModel>();
            }

            return clients;
        }
        #endregion

        #region ClientGet
        public List<ClientModel> GetClient(int IdClient)
        {
            var Client = new List<ClientModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    if (IdClient == 0)
                    {
                        query = "SELECT * FROM \"client\"";
                    }
                    else
                    {
                        query = "SELECT * FROM \"client\" WHERE id = @id";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (IdClient != 0)
                        {
                            cmd.Parameters.AddWithValue("@id", IdClient);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    var client = new ClientModel
                                    {
                                        id = reader.GetInt32(0),
                                        nameClient = reader.GetString(1),
                                        dateCreationClient = reader.GetDateTime(2),
                                        status = reader.GetBoolean(3),
                                        typeBusinessId = reader.GetInt32(4),
                                        corporateName = reader.IsDBNull(5) ? null : reader.GetString(5)                                    
                                    };
                                    Client.Add(client);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de negocio: {ex}");
                return new List<ClientModel>();
            }

            return Client;
        }
        #endregion

        #region ClientCreation
        public List<ClientModelInsert> InsertClient(ClientModelInsert clientModel)
        {
            var clients = new List<ClientModelInsert>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
            INSERT INTO ""client"" (""nameClient"", ""corporate_name"", ""dateCreationClient"", ""status"", ""typeBusinessId"")
            VALUES (@nameClient, @corporateName, @dateCreationClient, @status, @typeBusinessId)
            RETURNING *";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nameClient", clientModel.nameClient);
                        cmd.Parameters.AddWithValue("@corporateName", clientModel.corporateName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@dateCreationClient", DateTime.Now);
                        cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Bit).Value = clientModel.status ? "1" : "0";
                        cmd.Parameters.AddWithValue("@typeBusinessId", clientModel.typeBusinessId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var client = new ClientModelInsert
                                {
                                    nameClient = reader.GetString(1),
                                    corporateName = reader.IsDBNull(5) ? null : reader.GetString(5), // Manejo del campo corporate_name
                                    dateCreationClient = reader.GetDateTime(2),
                                    status = reader.GetBoolean(3),
                                    typeBusinessId = reader.GetInt32(4),
                                };
                                clients.Add(client);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar el cliente: {ex.Message}");
                return new List<ClientModelInsert>();
            }

            return clients;
        }
        #endregion

        #region TypeBusinessCreation
        public List<TypeBusinessModel> InsertTypeBusiness(TypeBusinessModel TypeBusiness)
        {
            var typebusiness = new List<TypeBusinessModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                INSERT INTO ""typeBusiness"" (""businessDescription"", ""status"")
                VALUES (@businessDescription, @status)
                RETURNING *";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@businessDescription", TypeBusiness.businessDescription);
                        cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Bit).Value = TypeBusiness.status ? "1" : "0";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var Type = new TypeBusinessModel
                                {
                                    businessDescription = reader.GetString(1),
                                    status = reader.GetBoolean(2),
                                };
                                typebusiness.Add(Type);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar el cliente: {ex.Message}");
                return new List<TypeBusinessModel>();
            }

            return typebusiness;
        }


        #endregion

        #region ClientUpdate
        public List<ClientModelInsert> ClientUpdate(ClientModelInsert clientModel, int IdClient)
        {
            var clients = new List<ClientModelInsert>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
            UPDATE ""client"" 
            SET 
                ""nameClient"" = @nameClient, 
                ""corporate_name"" = @corporateName,
                ""dateCreationClient"" = @dateCreationClient, 
                ""status"" = @status, 
                ""typeBusinessId"" = @typeBusinessId
            WHERE 
                 ""id"" = @id
            RETURNING *";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", IdClient);
                        cmd.Parameters.AddWithValue("@nameClient", clientModel.nameClient);
                        cmd.Parameters.AddWithValue("@corporateName", clientModel.corporateName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@dateCreationClient", DateTime.Now);
                        cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Bit).Value = clientModel.status ? "1" : "0";
                        cmd.Parameters.AddWithValue("@typeBusinessId", clientModel.typeBusinessId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var client = new ClientModelInsert
                                {
                                    nameClient = reader.GetString(1),
                                    corporateName = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    dateCreationClient = reader.GetDateTime(2),
                                    status = reader.GetBoolean(3),
                                    typeBusinessId = reader.GetInt32(4),
                                };
                                clients.Add(client);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el cliente: {ex.Message}");
                return new List<ClientModelInsert>();
            }

            return clients;
        }
        #endregion

        #region Headquarters
        public List<HeadQuartersModelGet> GetHeadquarters(int clientId, int idLocality)
        {
            if (clientId == -1 && idLocality == -1)
            {
                return new List<HeadQuartersModelGet>();
            }
            var Headquarter = new List<HeadQuartersModelGet>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    if (clientId == 0 && idLocality == 0)
                    {
                        query = @"
                SELECT h.id, h.""nameHeadquarter"", h.""numberPhone"", h.""address"", 
                       h.""localityId"", h.""clientId"", l.""nameLocality"", h.""status"", 
                       h.""email"", h.""signature_image"", h.""nit_cc""
                FROM ""headquarter"" h
                INNER JOIN ""client"" c ON h.""clientId"" = c.id
                INNER JOIN ""locality"" l ON h.""localityId"" = l.id
                ORDER BY ""nameHeadquarter"";";
                    }
                    else
                    {
                        query = @"
                SELECT h.id, h.""nameHeadquarter"", h.""numberPhone"", h.""address"", 
                       h.""localityId"", h.""clientId"", l.""nameLocality"", h.""status"", 
                       h.""email"", h.""signature_image"", h.""nit_cc""
                FROM ""headquarter"" h
                INNER JOIN ""client"" c ON h.""clientId"" = c.id
                INNER JOIN ""locality"" l ON h.""localityId"" = l.id
                WHERE h.""clientId"" = @clientId
                ORDER BY ""nameHeadquarter"";";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (clientId != 0 && idLocality != 0)
                        {
                            cmd.Parameters.AddWithValue("@clientId", clientId);
                            cmd.Parameters.AddWithValue("@localityId", idLocality);
                        }
                        else 
                        {
                            cmd.Parameters.AddWithValue("@clientId", clientId);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) &&
                                    !reader.IsDBNull(3) && !reader.IsDBNull(4) && !reader.IsDBNull(5) &&
                                    !reader.IsDBNull(6) && !reader.IsDBNull(7))
                                {
                                    var headquarter = new HeadQuartersModelGet
                                    {
                                        id = reader.GetInt32(0),
                                        nameHeadquarter = reader.GetString(1),
                                        numberPhone = reader.GetString(2),
                                        address = reader.GetString(3),
                                        localityId = reader.GetInt32(4),
                                        clientId = reader.GetInt32(5),
                                        localitiesData = new LocalitiesModelGet
                                        {
                                            nameLocality = reader.GetString(6),
                                        },
                                        status = reader.GetFieldValue<bool>(7),
                                        email = reader.IsDBNull(8) ? null :
                                                reader.GetString(8),
                                        signatureImage = reader.IsDBNull(9)
                                            ? null
                                            : Convert.ToBase64String((byte[])reader["signature_image"]),
                                        nitCc = reader.GetString(10)
                                    };

                                    Headquarter.Add(headquarter);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de negocio: {ex}");
                return new List<HeadQuartersModelGet>();
            }

            return Headquarter;
        }
        #endregion

        #region HeadquarterGet
        public List<HeadQuartersModelCreation> HeadquarterGet(int IdHeadQuarter)
        {
            var Headquarter = new List<HeadQuartersModelCreation>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    
                    query = @"
                SELECT 
                    id, 
                    ""nameHeadquarter"", 
                    ""numberPhone"", 
                    ""address"", 
                    ""dateCreationHeadquarter"", 
                    ""status"", 
                    ""clientId"", 
                    ""localityId"",
                    ""signature_image""
                FROM ""headquarter""
                WHERE id = @id;
            ";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", IdHeadQuarter);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Asegurarse de que todos los campos no son nulos
                                if (!reader.IsDBNull(reader.GetOrdinal("id")) &&
                                    !reader.IsDBNull(reader.GetOrdinal("nameHeadquarter")) &&
                                    !reader.IsDBNull(reader.GetOrdinal("numberPhone")) &&
                                    !reader.IsDBNull(reader.GetOrdinal("address")) &&
                                    !reader.IsDBNull(reader.GetOrdinal("dateCreationHeadquarter")) &&
                                    !reader.IsDBNull(reader.GetOrdinal("status")) &&
                                    !reader.IsDBNull(reader.GetOrdinal("clientId")) &&
                                    !reader.IsDBNull(reader.GetOrdinal("localityId")))
                                {
                                    var headquarter = new HeadQuartersModelCreation
                                    {
                                        nameHeadquarter = reader.GetString(reader.GetOrdinal("nameHeadquarter")),
                                        numberPhone = reader.GetString(reader.GetOrdinal("numberPhone")),
                                        address = reader.GetString(reader.GetOrdinal("address")),
                                        dateCreationHeadquarter = reader.GetDateTime(reader.GetOrdinal("dateCreationHeadquarter")),
                                        status = reader.GetBoolean(reader.GetOrdinal("status")),
                                        clientId = reader.GetInt32(reader.GetOrdinal("clientId")),
                                        localityId = reader.GetInt32(reader.GetOrdinal("localityId")),
                                        SignatureImage = reader.IsDBNull(reader.GetOrdinal("signature_image"))
                                            ? null
                                            : Convert.ToBase64String((byte[])reader["signature_image"]) 
                                    };

                                    Headquarter.Add(headquarter);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de negocio: {ex}");
                return new List<HeadQuartersModelCreation>();
            }

            return Headquarter;
        }
        #endregion

        #region HeadQuarterCreation
        public List<HeadQuartersModelCreation> HeadquartersCreation(HeadQuartersModelCreation HeadQuartersModel)
        {
            var headquartersmodel = new List<HeadQuartersModelCreation>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                INSERT INTO ""headquarter"" 
                (""nameHeadquarter"", ""numberPhone"", ""address"", ""dateCreationHeadquarter"", ""status"", ""clientId"", ""localityId"", ""signature_image"")
                VALUES (@nameHeadquarter, @numberPhone, @address, @dateCreationHeadquarter, @status, @clientId, @localityId, @signatureImage)
                RETURNING id, ""nameHeadquarter"", ""numberPhone"", ""address"", 
                          ""dateCreationHeadquarter"", ""status"", ""clientId"", ""localityId"", ""signature_image"";";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nameHeadquarter", HeadQuartersModel.nameHeadquarter);
                        cmd.Parameters.AddWithValue("@numberPhone", HeadQuartersModel.numberPhone);
                        cmd.Parameters.AddWithValue("@address", HeadQuartersModel.address);
                        cmd.Parameters.AddWithValue("@dateCreationHeadquarter", HeadQuartersModel.dateCreationHeadquarter);
                        cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Bit).Value = HeadQuartersModel.status ? "1" : "0";
                        cmd.Parameters.AddWithValue("@clientId", HeadQuartersModel.clientId);
                        cmd.Parameters.AddWithValue("@localityId", HeadQuartersModel.localityId);

                        
                        byte[] signatureImageBytes = string.IsNullOrEmpty(HeadQuartersModel.SignatureImage)
                            ? null
                            : Convert.FromBase64String(HeadQuartersModel.SignatureImage);

                        cmd.Parameters.AddWithValue("@signatureImage", (object)signatureImageBytes ?? DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var headquarter = new HeadQuartersModelCreation
                                {
                                    nameHeadquarter = reader.GetString(reader.GetOrdinal("nameHeadquarter")),
                                    numberPhone = reader.GetString(reader.GetOrdinal("numberPhone")),
                                    address = reader.GetString(reader.GetOrdinal("address")),
                                    dateCreationHeadquarter = reader.GetDateTime(reader.GetOrdinal("dateCreationHeadquarter")),
                                    status = reader.GetBoolean(reader.GetOrdinal("status")),
                                    clientId = reader.GetInt32(reader.GetOrdinal("clientId")),
                                    localityId = reader.GetInt32(reader.GetOrdinal("localityId")),
                                    SignatureImage = reader.IsDBNull(reader.GetOrdinal("signature_image"))
                                        ? null
                                        : Convert.ToBase64String((byte[])reader["signature_image"]) 
                                };
                                headquartersmodel.Add(headquarter);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar la sede: {ex.Message}");
                return new List<HeadQuartersModelCreation>();
            }

            return headquartersmodel;
        }
        #endregion

        #region HeadQuarterUpdate
        public List<HeadQuartersModelCreation> HeadQuarterUpdate(HeadQuartersModelCreation headQuarterModel, int idClient)
        {
            var headQuarters = new List<HeadQuartersModelCreation>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                UPDATE ""headquarter"" 
                SET 
                    ""nameHeadquarter"" = @nameHeadquarter,
                    ""numberPhone"" = @numberPhone, 
                    ""address"" = @address, 
                    ""dateCreationHeadquarter"" = @dateCreationHeadquarter, 
                    ""status"" = @status, 
                    ""clientId"" = @clientId, 
                    ""localityId"" = @localityId,
                    ""signature_image"" = @signatureImage,
                    ""email"" = @email
                WHERE 
                    ""id"" = @id
                RETURNING 
                    id, ""nameHeadquarter"", ""numberPhone"", ""address"", 
                    ""dateCreationHeadquarter"", ""status"", ""clientId"", 
                    ""localityId"", ""signature_image"", ""email"";
            ";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idClient);
                        cmd.Parameters.AddWithValue("@nameHeadquarter", headQuarterModel.nameHeadquarter);
                        cmd.Parameters.AddWithValue("@numberPhone", headQuarterModel.numberPhone);
                        cmd.Parameters.AddWithValue("@address", headQuarterModel.address);
                        cmd.Parameters.AddWithValue("@dateCreationHeadquarter", DateTime.Now);
                        cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Bit).Value = headQuarterModel.status ? (object)true : (object)false;
                        cmd.Parameters.AddWithValue("@clientId", headQuarterModel.clientId);
                        cmd.Parameters.AddWithValue("@localityId", headQuarterModel.localityId);
                        cmd.Parameters.AddWithValue("@email", headQuarterModel.email);

                        byte[] signatureImageBytes = null;
                        if (!string.IsNullOrEmpty(headQuarterModel.SignatureImage))
                        {
                            string cleanedBase64 = headQuarterModel.SignatureImage.Replace(" ", "").Replace("\n", "").Replace("\r", "");

                            if (IsBase64String(cleanedBase64))
                            {
                                try
                                {
                                    signatureImageBytes = Convert.FromBase64String(cleanedBase64);
                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine($"Error converting signature image to Base-64: {ex.Message}");
                                 
                                }
                            }
                            else
                            {
                                Console.WriteLine("The provided string is not a valid Base-64 string.");
                                // Handle the invalid Base64 string case
                            }
                        }

                        cmd.Parameters.AddWithValue("@signatureImage", (object)signatureImageBytes ?? DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var headQuarter = new HeadQuartersModelCreation
                                {
                                    nameHeadquarter = reader.GetString(reader.GetOrdinal("nameHeadquarter")),
                                    numberPhone = reader.GetString(reader.GetOrdinal("numberPhone")),
                                    address = reader.GetString(reader.GetOrdinal("address")),
                                    dateCreationHeadquarter = reader.GetDateTime(reader.GetOrdinal("dateCreationHeadquarter")),
                                    status = reader.GetBoolean(reader.GetOrdinal("status")),
                                    clientId = reader.GetInt32(reader.GetOrdinal("clientId")),
                                    localityId = reader.GetInt32(reader.GetOrdinal("localityId")),
                                    SignatureImage = reader.IsDBNull(reader.GetOrdinal("signature_image"))
                                        ? null
                                        : Convert.ToBase64String((byte[])reader["signature_image"])
                                };
                                headQuarters.Add(headQuarter);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la sede: {ex.Message}");
                return new List<HeadQuartersModelCreation>();
            }

            return headQuarters;
        }

        private bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64) || base64.Length % 4 != 0)
                return false;

            // Check for invalid characters
            foreach (char c in base64)
            {
                if (!char.IsLetterOrDigit(c) && c != '+' && c != '/' && c != '=')
                    return false;
            }

            return true;
        }
        #endregion

        #region UpdateSignature
        public bool UpdateSignature(int idHeadQuarter, string signatureImageBase64)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                UPDATE ""headquarter"" 
                SET ""signature_image"" = @signatureImage
                WHERE ""id"" = @idHeadQuarter";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idHeadQuarter", idHeadQuarter);

                        // Convertir la imagen en Base64 a un arreglo de bytes
                        byte[] signatureImageBytes = string.IsNullOrEmpty(signatureImageBase64)
                            ? null
                            : Convert.FromBase64String(signatureImageBase64);

                        cmd.Parameters.AddWithValue("@signatureImage", (object)signatureImageBytes ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0; // Devuelve true si se actualizó al menos un registro
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la firma: {ex.Message}");
                return false; // Indica que la actualización falló
            }
        }
        #endregion

        #region Measures
        public List<MeasuresModel> GetMeasures()
        {
            var Measures = new List<MeasuresModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    query = "select * from measure";


                    using (var cmd = new NpgsqlCommand(query, conn))
                    {

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) && !reader.IsDBNull(3))
                                {
                                    var measure = new MeasuresModel
                                    {
                                        id = reader.GetInt32(0),
                                        descriptionMeasures = reader.GetString(1),
                                        abbreviation = reader.GetString(2),
                                        status = reader.GetBoolean(3),

                                    };

                                    Measures.Add(measure);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de negocio: {ex}");
                return new List<MeasuresModel>();
            }

            return Measures;
        }

        #endregion

        #region MethodPayment
        public List<MethodPaymentModel> GetMethodPayment()
        {
            var MethodPayment = new List<MethodPaymentModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    query = "select * from \"methodPayment\"";


                    using (var cmd = new NpgsqlCommand(query, conn))
                    {

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                                {
                                    var methodpayment = new MethodPaymentModel
                                    {
                                        id = reader.GetInt32(0),
                                        descriptionPayment = reader.GetString(1),
                                        status = reader.GetBoolean(2),

                                    };

                                    MethodPayment.Add(methodpayment);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los métodos de pago: {ex}");
                return new List<MethodPaymentModel>();
            }

            return MethodPayment;
        }

        #endregion

        #region Products
        public List<ProductsModel> GetProducts()
        {
            var Products = new List<ProductsModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    query = "select * from \"product\"";


                    using (var cmd = new NpgsqlCommand(query, conn))
                    {

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                                {
                                    var products = new ProductsModel
                                    {
                                        id = reader.GetInt32(0),
                                        descriptionProduct = reader.GetString(1),
                                        abbreviation = reader.GetString(2),
                                        status = reader.GetBoolean(3),

                                    };

                                    Products.Add(products);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los productos: {ex}");
                return new List<ProductsModel>();
            }

            return Products;
        }

        #endregion

        #region TypeCollector
        public List<TypeCollectorsModel> GetTypeCollectors()
        {
            var typeCollectors = new List<TypeCollectorsModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    query = "select * from \"typeCollector\"";


                    using (var cmd = new NpgsqlCommand(query, conn))
                    {

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                                {
                                    var typecollectors = new TypeCollectorsModel
                                    {
                                        id = reader.GetInt32(0),
                                        descriptionCollector = reader.GetString(1),
                                        status = reader.GetBoolean(2),

                                    };

                                    typeCollectors.Add(typecollectors);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tipo de recolector: {ex}");
                return new List<TypeCollectorsModel>();
            }

            return typeCollectors;
        }

        #endregion

        #region CollectorCreation
        public List<CollectorModelInsert> InsertCollector(CollectorModelInsert collectorModel, loginCreationCollectorModel loginCollectorModel, byte[] imageData)
        {
            var collectors = new List<CollectorModelInsert>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    
                    string queryLogin = @"
                INSERT INTO resigrass.""loginCollector"" (""user"", ""password"", ""status"")
                VALUES (@user, @password, @status)
                RETURNING id";

                    int loginCollectorId;

                    using (var cmdLogin = new NpgsqlCommand(queryLogin, conn))
                    {
                    
                        string hashedPassword = HashPassword(loginCollectorModel.password);

                        cmdLogin.Parameters.AddWithValue("@user", loginCollectorModel.user);
                        cmdLogin.Parameters.AddWithValue("@password", hashedPassword);                                                                                  
                        var statusBit = loginCollectorModel.status ? new BitArray(new[] { true }) : new BitArray(new[] { false });

                    
                        cmdLogin.Parameters.AddWithValue("@status", statusBit);

                 
                        loginCollectorId = (int)cmdLogin.ExecuteScalar();
                    }

               
                    string queryCollector = @"
                INSERT INTO resigrass.collector (""nameCollector"", ""numberPhoneCollector"", ""dateCreationCollector"", ""status"", ""loginCollectorId"", ""typeCollectorId"", ""profile_image"", ""email"")
                VALUES (@nameCollector, @numberPhoneCollector, @dateCreationCollector, @status, @loginCollectorId, @typeCollectorId, @profileImage, @email)
                RETURNING *";

                    using (var cmdCollector = new NpgsqlCommand(queryCollector, conn))
                    {
                        cmdCollector.Parameters.AddWithValue("@nameCollector", collectorModel.nameCollector);
                        cmdCollector.Parameters.AddWithValue("@numberPhoneCollector", collectorModel.numberPhoneCollector);
                        cmdCollector.Parameters.AddWithValue("@dateCreationCollector", DateTime.Now);
                        var statusBit = collectorModel.status ? new BitArray(new[] { true }) : new BitArray(new[] { false });
                        cmdCollector.Parameters.AddWithValue("@status", statusBit);
                        cmdCollector.Parameters.AddWithValue("@loginCollectorId", loginCollectorId);
                        cmdCollector.Parameters.AddWithValue("@typeCollectorId", collectorModel.typeCollectorId);
                        cmdCollector.Parameters.AddWithValue("@email", collectorModel.emailCollector);

                     
                        cmdCollector.Parameters.AddWithValue("@profileImage", imageData ?? (object)DBNull.Value);

                        using (var reader = cmdCollector.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var collector = new CollectorModelInsert
                                {
                                    nameCollector = reader.GetString(1),
                                    numberPhoneCollector = reader.GetString(2),
                                    dateCreationCollector = reader.GetDateTime(3),
                                    status = reader.GetBoolean(4),
                                    loginCollectorId = reader.GetInt32(5),
                                    typeCollectorId = reader.GetInt32(6),
                                    emailCollector = reader.GetString(7)
                                };
                                collectors.Add(collector);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar el recolector: {ex.Message}");
                return new List<CollectorModelInsert>();
            }

            return collectors;
        }
        #endregion

        #region CollectorUpdate
        public bool UpdateCollector(int collectorId, CollectorModelUpdate collectorModel, byte[] imageData)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    // Construir la parte SET de la consulta dinámicamente
                    var setClauses = new List<string>
            {
                "\"nameCollector\" = @nameCollector",
                "\"numberPhoneCollector\" = @numberPhoneCollector",
                "\"status\" = @status",
                "\"profile_image\" = @profileImage"
            };

                    // Agregar "typeCollectorId" solo si no es 0
                    if (collectorModel.typeCollectorId != 0)
                    {
                        setClauses.Add("\"typeCollectorId\" = @typeCollectorId");
                    }

                    // Combinar la consulta
                            string queryUpdate = $@"
                    UPDATE resigrass.collector
                    SET {string.Join(", ", setClauses)}
                    WHERE ""id"" = @collectorId";

                    using (var cmdUpdate = new NpgsqlCommand(queryUpdate, conn))
                    {
                        // Parámetros obligatorios
                        cmdUpdate.Parameters.AddWithValue("@collectorId", collectorId);
                        cmdUpdate.Parameters.AddWithValue("@nameCollector", collectorModel.nameCollector);
                        cmdUpdate.Parameters.AddWithValue("@numberPhoneCollector", collectorModel.numberPhoneCollector);

                        var statusBit = collectorModel.status ? new BitArray(new[] { true }) : new BitArray(new[] { false });
                        cmdUpdate.Parameters.AddWithValue("@status", statusBit);

                        cmdUpdate.Parameters.AddWithValue("@profileImage", imageData ?? (object)DBNull.Value);

                        // Parámetro opcional: typeCollectorId (solo si no es 0)
                        if (collectorModel.typeCollectorId != 0)
                        {
                            cmdUpdate.Parameters.AddWithValue("@typeCollectorId", collectorModel.typeCollectorId);
                        }

                        // Ejecutar la consulta
                        int rowsAffected = cmdUpdate.ExecuteNonQuery();
                        return rowsAffected > 0; // Retorna true si al menos una fila fue afectada
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el recolector: {ex.Message}");
                return false; // Retorna false si ocurrió un error
            }
        }
        #endregion

        #region HashPassword
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        } 
        #endregion

        #region CollectorLoginGet
        public LoginResponse CollectorLoginGet(loginCreationCollectorModelValidate LoginCollector)
        {
            var response = new LoginResponse();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string queryCheckUser = "SELECT * FROM \"loginCollector\" WHERE \"user\" = @user";
                    using (var cmdCheckUser = new NpgsqlCommand(queryCheckUser, conn))
                    {
                        cmdCheckUser.Parameters.AddWithValue("@user", LoginCollector.user);

                        using (var reader = cmdCheckUser.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                response.Success = false;
                                response.Message = "El usuario no existe.";
                                return response;
                            }

                            var storedPassword = reader.GetString(reader.GetOrdinal("password"));
                            var storedStatus = reader.GetBoolean(reader.GetOrdinal("status"));

                            reader.Close();

    
                            string hashedInputPassword = HashPassword(LoginCollector.password);
                            if (hashedInputPassword != storedPassword)
                            {
                                response.Success = false;
                                response.Message = "La contraseña es incorrecta.";
                                return response;
                            }

                            string queryCollector = @"
                            SELECT c.id, c.""nameCollector"", c.""numberPhoneCollector"", 
                                   tc.""descriptionCollector"", c.""profile_image"", c.""status""
                            FROM collector c
                            INNER JOIN ""typeCollector"" tc ON c.""typeCollectorId"" = tc.id
                            INNER JOIN ""loginCollector"" lc ON c.""loginCollectorId"" = lc.id
                            WHERE lc.""user"" = @user";

                            using (var cmdCollector = new NpgsqlCommand(queryCollector, conn))
                            {
                                cmdCollector.Parameters.AddWithValue("@user", LoginCollector.user);

                                using (var collectorReader = cmdCollector.ExecuteReader())
                                {

                                    
                                    if (collectorReader.Read())
                                    {
                                        var storedStatus2 = collectorReader.GetBoolean(collectorReader.GetOrdinal("status"));



                                        if (storedStatus2 == false)
                                        {
                                            response.Success = false;
                                            response.Message = "El usuario no se encuentra habilitado.";
                                            return response;
                                        }
                                        var collectorId = collectorReader.GetInt32(0);

                                        var collectorData = new CollectorsModelSelect
                                        {
                                            id = collectorId,
                                            nameCollector = collectorReader.GetString(1),
                                            numberPhoneCollector = collectorReader.GetString(2),
                                            typeCollectorsModelId = new TypeCollectorsModelSelect
                                            {
                                                descriptionCollector = collectorReader.GetString(3),
                                            },
                                            status = collectorReader.GetBoolean(5),
                                        };

                                        
                                        var profileImageBytes = collectorReader.IsDBNull(4) ? null : (byte[])collectorReader[4];

                                        collectorData.profile_image = profileImageBytes != null ? Convert.ToBase64String(profileImageBytes) : null;

                                        collectorReader.Close();

                                        string serialQuery = @"
                                        SELECT ""serial_number""
                                        FROM collection
                                        WHERE ""collectorId"" = @collectorId
                                        ORDER BY id DESC
                                        LIMIT 1";

                                        using (var serialCmd = new NpgsqlCommand(serialQuery, conn))
                                        {
                                            serialCmd.Parameters.AddWithValue("@collectorId", collectorId);

                                            var result = serialCmd.ExecuteScalar();
                                            string nextSerial;

                                            if (result != null)
                                            {
                                                var lastSerial = result.ToString();
                                                var lastNumber = int.Parse(lastSerial.Split('-').Last());
                                                nextSerial = $"{(lastNumber + 1):D1}";
                                            }
                                            else
                                            {
                                                nextSerial = $"1";
                                            }
                                            collectorData.nextSerialNumber = nextSerial;
                                        }

                                        response.Data = collectorData;
                                        response.Success = true;
                                        response.Message = "Inicio de sesión exitoso.";
                                    }
                                    else
                                    {
                                        response.Success = false;
                                        response.Message = "Datos del recolector no encontrados.";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error al obtener los recolectores: {ex.Message}";
            }

            return response;
        }
        #endregion

        #region InsertCollection
        public List<RecolectionModelInsert> InsertCollection(RecolectionModelInsert CollectionModel)
        {
            var Collection = new List<RecolectionModelInsert>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    CollectionModel.endDate = CollectionModel.receivedDate.AddMonths(1);
                    conn.Open();

                    string query = @"
                INSERT INTO collection (
                    ""receivedDate"", ""endDate"", ""fullPayment"", ""priceUnit"", ""netWeight"",
                    observations, ""receivedFull"", ""bowlEmpty"", ""collectorId"", 
                    ""headquarterId"", ""measureId"", ""methodPaymentId"", ""productId"", ""serial_number"")
                VALUES (
                    @receivedDate, @endDate, @fullPayment, @priceUnit, @netWeight, 
                    @observations, @receivedFull, @bowlEmpty, @collectorId, 
                    @headquarterId, @measureId, @methodPaymentId, @productId, @serialNumber)
                RETURNING *";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@receivedDate", CollectionModel.receivedDate);
                        cmd.Parameters.AddWithValue("@endDate", CollectionModel.endDate);
                        cmd.Parameters.AddWithValue("@fullPayment", CollectionModel.fullPayment);
                        cmd.Parameters.AddWithValue("@priceUnit", CollectionModel.priceUnit);
                        cmd.Parameters.AddWithValue("@netWeight", CollectionModel.netWeight);
                        cmd.Parameters.AddWithValue("@observations", CollectionModel.observations);
                        cmd.Parameters.AddWithValue("@receivedFull", CollectionModel.receivedFull);
                        cmd.Parameters.AddWithValue("@bowlEmpty", CollectionModel.bowlEmpty);
                        cmd.Parameters.AddWithValue("@collectorId", CollectionModel.collectorId);
                        cmd.Parameters.AddWithValue("@headquarterId", CollectionModel.headquarterId);
                        cmd.Parameters.AddWithValue("@measureId", CollectionModel.measureId);
                        cmd.Parameters.AddWithValue("@methodPaymentId", CollectionModel.methodPaymentId);
                        cmd.Parameters.AddWithValue("@productId", CollectionModel.productId);
                        cmd.Parameters.AddWithValue("@serialNumber", CollectionModel.serial_number);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var collection = new RecolectionModelInsert
                                {
                                    receivedDate = reader.GetDateTime(1),
                                    endDate = reader.GetDateTime(2),
                                    fullPayment = reader.GetFloat(3),
                                    priceUnit = reader.GetFloat(4),
                                    netWeight = reader.GetFloat(5),
                                    observations = reader.GetString(6),
                                    receivedFull = reader.GetInt32(7),
                                    bowlEmpty = reader.GetInt32(8),
                                    collectorId = reader.GetInt32(9),
                                    headquarterId = reader.GetInt32(10),
                                    measureId = reader.GetInt32(11),
                                    methodPaymentId = reader.GetInt32(12),
                                    productId = reader.GetInt32(13), 
                                };
                                Collection.Add(collection);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar el cliente: {ex.Message}");
                return new List<RecolectionModelInsert>();
            }

            return Collection;
        }


        #endregion

        #region GetAllCollections
        public List<RecolectionModelStat> GetAllCollections()
        {
            var collections = new List<RecolectionModelStat>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
            SELECT 
                c.""id"" AS Id,
                cl.""id"" AS ClienteId, cl.""nameClient"" AS nameClient,
                lo.""nameLocality"" AS City,
                cl.""nitCc"" AS Nit,
                h.""address"" AS Address,
                h.""numberPhone"" AS Phone,
                cl.""typeBusinessId"" AS BusinessType,
                h.""id"" AS HeadquarterId, h.""nameHeadquarter"" AS NameHeadquarter,
                c.""receivedDate"" AS Date,
                c.""netWeight"" AS Cantidad,
                m.""id"" AS MedidaId, m.""abbreviation"" AS MedidaNombre,
                col.""id"" AS RecolectorId, col.""nameCollector"" AS RecolectorNombre,
                c.""fullPayment"" AS Pago,
                c.""observations"" AS Observaciones,
                c.""serial_number"" AS Serial,
                c.""is_sent"" AS IsSent,
                cl.""corporate_name"" AS TipoDeNegocio,
                h.""signature_image"" AS Firma
            FROM collection c
            INNER JOIN headquarter h ON c.""headquarterId"" = h.""id""
            INNER JOIN client cl ON h.""clientId"" = cl.""id""
            INNER JOIN measure m ON c.""measureId"" = m.""id""
            INNER JOIN collector col ON c.""collectorId"" = col.""id""
            INNER JOIN locality lo ON h.""localityId"" = lo.""id""
            ORDER BY c.""serial_number""";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var collection = new RecolectionModelStat
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                    nameClient = reader.GetString(reader.GetOrdinal("nameClient")),
                                    City = reader.GetString(reader.GetOrdinal("City")),
                                    NIT = reader.GetString(reader.GetOrdinal("Nit")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                    BusinessType = reader.GetInt32(reader.GetOrdinal("BusinessType")),
                                    HeadquarterId = reader.GetInt32(reader.GetOrdinal("HeadquarterId")),
                                    NameHeadquarter = reader.GetString(reader.GetOrdinal("NameHeadquarter")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                    Amount = reader.GetFloat(reader.GetOrdinal("Cantidad")),
                                    MeasureId = reader.GetInt32(reader.GetOrdinal("MedidaId")),
                                    NameMeasure = reader.GetString(reader.GetOrdinal("MedidaNombre")),
                                    CollectorId = reader.GetInt32(reader.GetOrdinal("RecolectorId")),
                                    NameCollector = reader.GetString(reader.GetOrdinal("RecolectorNombre")),
                                    fullPayment = reader.GetFloat(reader.GetOrdinal("Pago")),
                                    Observations = reader.IsDBNull(reader.GetOrdinal("Observaciones"))
                                                   ? null
                                                   : reader.GetString(reader.GetOrdinal("Observaciones")),
                                    Serial = reader.GetString(reader.GetOrdinal("Serial")),
                                    IsSent = reader.GetBoolean(reader.GetOrdinal("IsSent")),
                                    BusinessTypeName = reader.IsDBNull(reader.GetOrdinal("TipoDeNegocio"))
                                                       ? null
                                                       : reader.GetString(reader.GetOrdinal("TipoDeNegocio")),
                                    SignatureImage = reader.IsDBNull(reader.GetOrdinal("Firma"))
                                                     ? null
                                                     : Convert.ToBase64String((byte[])reader["Firma"])
                                };

                                collections.Add(collection);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las recolecciones: {ex.Message}");
            }

            return collections;
        }
        #endregion

        #region GetWeeklyOilByDateRange
        public List<WeeklyOilData> GetWeeklyOilByDateRange(DateTime startDate, DateTime endDate)
        {
            var weeklyOilData = new List<WeeklyOilData>();

            try
            {

                startDate = startDate.Date;
                endDate = endDate.Date;

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                                SELECT 
                                    DATE_TRUNC('week', c.""receivedDate"") AS WeekStart,
                                    COALESCE(SUM(c.""netWeight""), 0) AS TotalOil,
                                    (SELECT COALESCE(SUM(c2.""netWeight""), 0) FROM collection c2
                                     INNER JOIN headquarter h2 ON c2.""headquarterId"" = h2.""id""
                                     WHERE h2.""is_certified"" = B'1'
	                                 AND DATE(c2.""receivedDate"") BETWEEN DATE(@startDate) AND DATE(@endDate))
	                                 AS TotalOilAllWeeks
                                FROM collection c
                                INNER JOIN headquarter h ON c.""headquarterId"" = h.""id""
                                AND DATE(c.""receivedDate"") BETWEEN DATE(@startDate) AND DATE(@endDate)
                                AND h.""is_certified"" = B'1'
                                GROUP BY WeekStart
                                ORDER BY WeekStart;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var weekStart = reader.GetDateTime(reader.GetOrdinal("WeekStart"));
                                var totalOil = reader.GetDouble(reader.GetOrdinal("TotalOil"));
                                var totalOilAllWeeks = reader.GetDouble(reader.GetOrdinal("TotalOilAllWeeks"));

                                weeklyOilData.Add(new WeeklyOilData
                                {
                                    WeekStart = weekStart,
                                    WeekEnd = weekStart.AddDays(6), // El fin de la semana es 6 días después
                                    TotalOil = totalOil,
                                    TotalOilAllWeeks = totalOilAllWeeks
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el total de aceite por rango de fechas: {ex.Message}");
            }

            return weeklyOilData;
        } 
        #endregion

        #region UpdateCollection
        public bool UpdateCollection(int collectionId, float newPayment, float newWeight)
        {
            bool isUpdated = false;

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                UPDATE collection
                SET ""fullPayment"" = @newPayment, 
                    ""netWeight"" = @newWeight
                WHERE ""id"" = @collectionId";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@newPayment", newPayment);
                        cmd.Parameters.AddWithValue("@newWeight", newWeight);
                        cmd.Parameters.AddWithValue("@collectionId", collectionId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        isUpdated = rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la recolección: {ex.Message}");
            }

            return isUpdated; 
        }
        #endregion

        #region GetAllCollectors
        public List<CollectorsModel> AllCollectorsGet()
        {
            var Collectors = new List<CollectorsModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                                    SELECT id, ""nameCollector"", ""numberPhoneCollector"", ""dateCreationCollector"", status, ""loginCollectorId"", ""typeCollectorId"", profile_image
	                                FROM resigrass.collector
                                    ORDER BY id asc;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var collection = new CollectorsModel
                                {
                                    id = reader.GetInt32(0),
                                    nameCollector = reader.GetString(1),
                                    numberPhoneCollector = reader.GetString(2),
                                    dateCreationCollector = reader.GetDateTime(3),
                                    status = reader.GetBoolean(4)
                                   };

                                var profileImageBytes = reader.IsDBNull(7) ? null : (byte[])reader[7];

                                collection.profile_image = profileImageBytes != null ? Convert.ToBase64String(profileImageBytes) : null;
                                Collectors.Add(collection);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las recolecciones: {ex.Message}");
            }

            return Collectors;
        }
        #endregion

        #region Records
        public List<RecolectionModel> GetRecordsDueInTwoDays()
        {
            var records = new List<RecolectionModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        string query = @"
                            SELECT c.""serial_number"", c.""receivedDate"", c.""endDate"", c.""fullPayment"", c.""priceUnit"", c.""netWeight"", 
                                   c.""observations"", c.""receivedFull"", c.""bowlEmpty"", c.""collectorId"", 
                                   c.""headquarterId"", c.""measureId"", c.""methodPaymentId"", c.""productId"", 
                                   h.email, cl.""nameClient"", lo.""nameLocality"", h.""numberPhone""
                            FROM collection c
                            INNER JOIN headquarter h ON c.""headquarterId"" = h.id
                            INNER JOIN client cl ON cl.id = h.""clientId""
                            INNER JOIN locality lo ON lo.id = h.""localityId""
                            WHERE c.""receivedDate""::date = (NOW() - INTERVAL '2 days')::date AND c.""is_sent"" = B'0';";

                        using (var cmd = new NpgsqlCommand(query, conn, transaction))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var record = new RecolectionModel
                                    {
                                        serial_number = reader.GetString(0),
                                        receivedDate = reader.GetDateTime(1),
                                        endDate = reader.GetDateTime(2),
                                        fullPayment = reader.GetFloat(3),
                                        priceUnit = reader.GetFloat(4),
                                        netWeight = reader.GetFloat(5),
                                        observations = reader.GetString(6),
                                        receivedFull = reader.GetInt32(7),
                                        bowlEmpty = reader.GetInt32(8),
                                        collectorId = reader.GetInt32(9),
                                        headquarterId = reader.GetInt32(10),
                                        measureId = reader.GetInt32(11),
                                        methodPaymentId = reader.GetInt32(12),
                                        productId = reader.GetInt32(13),
                                        email = reader.IsDBNull(14) ? null : reader.GetString(14),
                                        nameClient = reader.GetString(15),
                                        nameLocality = reader.GetString(16),
                                        numberPhone = reader.GetString(17)
                                    };

                                    records.Add(record);
                                }
                            }
                        }
                        foreach (var record in records)
                        {
                            using (var updateCmd = new NpgsqlCommand("UPDATE collection SET is_sent = B'1' WHERE id = @id", conn, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@id", record.id);
                                updateCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener y actualizar los registros: {ex.Message}");
                return new List<RecolectionModel>();
            }

            return records;
        }
        #endregion

        #region GetRecolectionById
        public RecolectionModel GetRecolectionById(int id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    c.""id"", 
                    c.""receivedDate"", 
                    c.""endDate"", 
                    c.""fullPayment"", 
                    c.""priceUnit"", 
                    c.""netWeight"", 
                    c.""observations"", 
                    c.""receivedFull"", 
                    c.""bowlEmpty"", 
                    c.""collectorId"", 
                    col.""nameCollector"" AS CollectedName, 
                    c.""headquarterId"", 
                    h.""email"", 
                    h.""address"", 
                    h.""numberPhone"" AS HeadquarterPhone, 
                    cl.""typeBusinessId"", 
                    tb.""businessDescription"" AS BusinessType,
                    h.""nameHeadquarter"", -- Nombre de la sede
                    cl.""nitCc"",
                    cl.""nameClient"",
                    c.""serial_number"",
                    lo.""nameLocality"",
                    h.signature_image
                FROM collection c
                INNER JOIN headquarter h ON c.""headquarterId"" = h.""id""
                INNER JOIN client cl ON h.""clientId"" = cl.""id""
                INNER JOIN ""typeBusiness"" tb ON cl.""typeBusinessId"" = tb.""id""
                INNER JOIN collector col ON c.""collectorId"" = col.""id""
                INNER JOIN locality lo ON lo.id = h.""localityId""
                WHERE c.""id"" = @id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                byte[] signatureImageBytes = null;
                                if (!reader.IsDBNull(reader.GetOrdinal("signature_image")))
                                {
                                    signatureImageBytes = new byte[reader.GetBytes(reader.GetOrdinal("signature_image"), 0, null, 0, int.MaxValue)];
                                    reader.GetBytes(reader.GetOrdinal("signature_image"), 0, signatureImageBytes, 0, signatureImageBytes.Length);
                                }

                                return new RecolectionModel
                                {
                                    receivedDate = reader.GetDateTime(reader.GetOrdinal("receivedDate")),
                                    endDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                                    fullPayment = reader.GetFloat(reader.GetOrdinal("fullPayment")),
                                    priceUnit = reader.GetFloat(reader.GetOrdinal("priceUnit")),
                                    netWeight = reader.GetFloat(reader.GetOrdinal("netWeight")),
                                    observations = reader.IsDBNull(reader.GetOrdinal("observations"))
                                                   ? null
                                                   : reader.GetString(reader.GetOrdinal("observations")),
                                    receivedFull = reader.GetInt32(reader.GetOrdinal("receivedFull")),
                                    bowlEmpty = reader.GetInt32(reader.GetOrdinal("bowlEmpty")),
                                    collectorId = reader.GetInt32(reader.GetOrdinal("collectorId")),
                                    collectedName = reader.GetString(reader.GetOrdinal("CollectedName")), // Nombre del recolector
                                    headquarterId = reader.GetInt32(reader.GetOrdinal("headquarterId")),
                                    email = reader.IsDBNull(reader.GetOrdinal("email"))
                                            ? null
                                            : reader.GetString(reader.GetOrdinal("email")),
                                    address = reader.GetString(reader.GetOrdinal("address")),
                                    numberPhone = reader.GetString(reader.GetOrdinal("HeadquarterPhone")),
                                    businessTypeId = reader.GetInt32(reader.GetOrdinal("typeBusinessId")),
                                    businessType = reader.GetString(reader.GetOrdinal("BusinessType")), // Tipo de negocio
                                    nameClient = reader.GetString(reader.GetOrdinal("nameHeadquarter")), // Nombre de la sede
                                    nitCc = reader.GetString(reader.GetOrdinal("nitCc")), // NIT del cliente
                                    serial_number = reader.GetString(reader.GetOrdinal("serial_number")),
                                    nameLocality = reader.GetString(reader.GetOrdinal("nameLocality")),
                                    signature_image = signatureImageBytes
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la recolección: {ex.Message}");
            }

            return null;
        }

        public void MarkAsSent(int id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string updateQuery = "UPDATE collection SET is_sent = B'1' WHERE id = @id";

                    using (var cmd = new NpgsqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el estado de envío: {ex.Message}");
            }
        } 
        #endregion

        #region UserAdminCreation
        public string UserAdminCreation(userAdminModel userAdminModel)
        {
            var Users = new List<userAdminModel>();
            string hashedPassword;
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    // Primero insertar en la tabla loginCollector
                    string query = @"

            select * from  ""adminUsers"" where ""user""= @user";



                    int loginCollectorId;

                    using (var cmdLogin = new NpgsqlCommand(query, conn))

                    {
                        cmdLogin.Parameters.AddWithValue("@user", userAdminModel.user);

                        using (var reader = cmdLogin.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return "Ya existe usuario";
                            }

                            // Hash de la contraseña antes de la inserción
                            hashedPassword = HashPassword(userAdminModel.password);

                            //cmdLogin.Parameters.AddWithValue("@user", userAdminModel.user);
                            //cmdLogin.Parameters.AddWithValue("@password", hashedPassword); // Usar la contraseña hasheada                                                                                      
                            //var statusBit = userAdminModel.status ? new BitArray(new[] { true }) : new BitArray(new[] { false });

                            // Asignar el valor del parámetro como un array de bits
                            //cmdLogin.Parameters.AddWithValue("@status", statusBit);
                        }
                        string QueryUserCreation = @"
            INSERT INTO resigrass.""adminUsers""(name, ""user"", password, ""phoneNumber"", ""profileId"")
            VALUES (@name,@user,@password,@phoneNumber,@profileId);";

                        using (var cmdUsers = new NpgsqlCommand(QueryUserCreation, conn))
                        {
                            cmdUsers.Parameters.AddWithValue("@name", userAdminModel.name);
                            cmdUsers.Parameters.AddWithValue("@user", userAdminModel.user);
                            cmdUsers.Parameters.AddWithValue("@password", hashedPassword);
                            cmdUsers.Parameters.AddWithValue("@phoneNumber", userAdminModel.phoneNumber);
                            cmdUsers.Parameters.AddWithValue("@profileId", userAdminModel.profileId);


                            using (var reader = cmdUsers.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    return "Usuario creado :D";

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar el recolector: {ex.Message}");
                return "";
            }

            return "collectors";
        }



        #endregion

        #region AdminLoginGet
        public LoginResponse UserAdminLogin(userAdminLoginModel UserModel)
        {
            var response = new LoginResponse();
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryCheckUser = "SELECT * FROM \"adminUsers\" WHERE \"user\" = @user";
                    using (var cmdCheckUser = new NpgsqlCommand(queryCheckUser, conn))
                    {
                        cmdCheckUser.Parameters.AddWithValue("@user", UserModel.user);

                        using (var reader = cmdCheckUser.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                response.Success = false;
                                response.Message = "El usuario no existe.";
                                return response;
                            }

                            var storedPassword = reader.GetString(reader.GetOrdinal("password"));
                            //var storedStatus = reader.GetBoolean(reader.GetOrdinal("status")); 

                            //if (!storedStatus)
                            //{
                            //    response.Success = false;
                            //    response.Message = "El usuario no está habilitado.";
                            //    return response;
                            //}

                            reader.Close();

                            string hashedInputPassword = HashPassword(UserModel.password);
                            if (hashedInputPassword != storedPassword)
                            {
                                response.Success = false;
                                response.Message = "La contraseña es incorrecta.";
                                return response;
                            }

                            response.Success = true;
                            response.Message = "Inicio de sesión exitoso.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error al obtener los recolectores: {ex.Message}";
            }

            return response;
        }
        #endregion

        #region GetWhatsappNumber
        public string GetWhatsappNumber()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryCheckUser = @"WITH last_sent AS (
                                                  SELECT id
                                                  FROM whatsapp_number
                                                  WHERE is_last_sent = B'1'
                                                  LIMIT 1
                                                ),
                                                next_sent AS (
                                                  SELECT id
                                                  FROM whatsapp_number
                                                  WHERE id > (SELECT id FROM last_sent)
                                                  ORDER BY id ASC
                                                  LIMIT 1
                                                )
                                                UPDATE whatsapp_number
                                                SET is_last_sent = 
                                                  CASE
                                                    WHEN id = COALESCE((SELECT id FROM next_sent), (SELECT id FROM whatsapp_number ORDER BY id ASC LIMIT 1)) THEN B'1'
                                                    ELSE B'0'
                                                  END;

                                                SELECT number FROM whatsapp_number
                                                WHERE is_last_sent = B'1'";

                    using (var cmdCheckUser = new NpgsqlCommand(queryCheckUser, conn))
                    {
                        using (var reader = cmdCheckUser.ExecuteReader())
                        {
                            if (!reader.Read())
                                return "Error";
                            return reader.GetString(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }
        #endregion

        #region GetPinCollector
        public int GetPinCollector(int idUser)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryCheckUser = @"SELECT pin FROM pin_collector
                                                WHERE id_collector = @idUser";

                    using (var cmdCheckUser = new NpgsqlCommand(queryCheckUser, conn))
                    {
                        cmdCheckUser.Parameters.AddWithValue("@idUser", idUser);

                        using (var reader = cmdCheckUser.ExecuteReader())
                        {
                            if (!reader.Read())
                                return 0;
                            return reader.GetInt32(reader.GetOrdinal("pin"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        #region #region GetDataCollector
        public DataCollector GetDataCollector(int idUser)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryCheckUser = @"SELECT 
	                                            profile_image, 
	                                            ""nameCollector"", 
	                                            ""numberPhoneCollector"", 
	                                            ""dateCreationCollector"" 
                                            FROM resigrass.collector
                                            WHERE id = @idUser";

                    using (var cmdCheckUser = new NpgsqlCommand(queryCheckUser, conn))
                    {
                        cmdCheckUser.Parameters.AddWithValue("@idUser", idUser);

                        using (var reader = cmdCheckUser.ExecuteReader())
                        {
                            if (!reader.Read())
                                return new DataCollector();

                            return new DataCollector
                            {
                                profile_image = reader.IsDBNull(0) ? null : Convert.ToBase64String((byte[])reader[0]),
                                nameCollector = reader.GetString(1),
                                numberPhoneCollector = reader.GetString(2),
                                dateCreationCollector = reader.GetDateTime(3).ToString("dd/MM/yyyy")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new DataCollector();
            }
        }
        #endregion

        #region UpdateTokenUser
        public async Task<bool> UpdateTokenUser(int idUser, string token)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"UPDATE pin_collector
                                        SET pin = @token, 
                                            date = CURRENT_DATE
                                        WHERE id_collector = @idUser;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@token", int.Parse(token));
                        cmd.Parameters.AddWithValue("@idUser", idUser);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la recolección: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region GetUsersToSendToken
        public async Task<List<DataUpdateToken>> GetUsersToSendToken()
        {
            List<DataUpdateToken> users = new List<DataUpdateToken>();
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryCheckUser = @"SELECT c.id, CAST(p.pin AS TEXT) AS token, c.email
                                                FROM pin_collector p
                                                INNER JOIN collector c ON p.id_collector = c.id";

                    using (var cmdCheckUser = new NpgsqlCommand(queryCheckUser, conn))
                    {
                        using (var reader = cmdCheckUser.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new DataUpdateToken
                                {
                                    id = reader.GetInt32(0),
                                    token = reader.GetString(1),
                                    email = reader.GetString(2)
                                });
                            }
                        }
                    }
                }
                return users;
            }
            catch
            {
                return users;
            }
        }
        #endregion
    }
}