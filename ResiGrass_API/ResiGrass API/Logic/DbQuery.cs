using Npgsql;
using ResiGrass_API.Models;

namespace ResiGrass_API.Logic
{
    public class DbQuery
    {
        private readonly string _connectionString;

        public DbQuery(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Municipalities
        public List<MunicipalityModel> GetMunicipalities()
        {
            var municipalities = new List<MunicipalityModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("SELECT * FROM municipality", conn))
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

                    using (var cmd = new NpgsqlCommand("SELECT *  FROM locality INNER JOIN municipality on locality.\"municipalityId\" = municipality.id WHERE \"municipalityId\" = @idMunicipality", conn))
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
                                        status = reader.GetBoolean(2),
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
                                        businnessDescription = reader.GetString(1),
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
            var Client = new List<ClientModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    if (idTypeBusiness == 0)
                    {
                        query = "SELECT * FROM \"client\" INNER JOIN \"typeBusiness\" ON \"client\".\"typeBusinessId\" = \"typeBusiness\".id";
                    }
                    else
                    {
                        query = "SELECT * FROM \"client\" INNER JOIN \"typeBusiness\" ON \"client\".\"typeBusinessId\" = \"typeBusiness\".id WHERE \"typeBusinessId\" = @idTypeBusiness";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (idTypeBusiness != 0)
                        {
                            cmd.Parameters.AddWithValue("idTypeBusiness", idTypeBusiness);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) && !reader.IsDBNull(3) && !reader.IsDBNull(4) && !reader.IsDBNull(5) && !reader.IsDBNull(6) && !reader.IsDBNull(7) && !reader.IsDBNull(8))
                                {
                                    var client = new ClientModel
                                    {
                                        id = reader.GetInt32(0),
                                        nitCc = reader.GetString(1),
                                        nameClient = reader.GetString(2),
                                        dateCreationClient = reader.GetDateTime(3),
                                        sign = reader.GetString(4),
                                        status = reader.GetBoolean(5),
                                        typeBusinessId = reader.GetInt32(6),
                                        businessModelData = new TypeBusinessModel
                                        {
                                            id = reader.GetInt32(7),
                                            businnessDescription = reader.GetString(8),
                                            status = reader.GetBoolean(9),
                                        }
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


                    query = "SELECT * FROM \"client\" WHERE id = @id";


                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", IdClient);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) && !reader.IsDBNull(3) && !reader.IsDBNull(4) && !reader.IsDBNull(5) && !reader.IsDBNull(6))
                                {
                                    var client = new ClientModel
                                    {
                                        id = reader.GetInt32(0),
                                        nitCc = reader.GetString(1),
                                        nameClient = reader.GetString(2),
                                        dateCreationClient = reader.GetDateTime(3),
                                        sign = reader.GetString(4),
                                        status = reader.GetBoolean(5),
                                        typeBusinessId = reader.GetInt32(6),
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
                INSERT INTO ""client"" (""nitCc"", ""nameClient"", ""dateCreationClient"", ""sign"", ""status"", ""typeBusinessId"")
                VALUES (@nitCc, @nameClient, @dateCreationClient, @sign, @status, @typeBusinessId)
                RETURNING *";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nitCc", clientModel.nitCc);
                        cmd.Parameters.AddWithValue("@nameClient", clientModel.nameClient);
                        cmd.Parameters.AddWithValue("@dateCreationClient", DateTime.Now);
                        cmd.Parameters.AddWithValue("@sign", clientModel.sign);
                        cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Bit).Value = clientModel.status ? "1" : "0";
                        //cmd.Parameters.AddWithValue("@status", clientModel.status ? "B'1'" : "B'0'");
                        cmd.Parameters.AddWithValue("@typeBusinessId", clientModel.typeBusinessId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var client = new ClientModelInsert
                                {
                                    nitCc = reader.GetString(1),
                                    nameClient = reader.GetString(2),
                                    dateCreationClient = reader.GetDateTime(3),
                                    sign = reader.GetString(4),
                                    status = reader.GetBoolean(5),
                                    typeBusinessId = reader.GetInt32(6),

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
                ""nitCc"" = @nitCc,
                ""nameClient"" = @nameClient, 
                ""dateCreationClient"" = @dateCreationClient, 
                ""sign"" = @sign, 
                ""status"" = @status, 
                ""typeBusinessId"" = @typeBusinessId
            WHERE 
                 ""id"" = @id
            RETURNING *";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@id", IdClient);
                        cmd.Parameters.AddWithValue("@nitCc", clientModel.nitCc);
                        cmd.Parameters.AddWithValue("@nameClient", clientModel.nameClient);
                        cmd.Parameters.AddWithValue("@dateCreationClient", DateTime.Now);
                        cmd.Parameters.AddWithValue("@sign", clientModel.sign);
                        cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Bit).Value = clientModel.status ? "1" : "0";
                        cmd.Parameters.AddWithValue("@typeBusinessId", clientModel.typeBusinessId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var client = new ClientModelInsert
                                {
                                    nitCc = reader.GetString(1),
                                    nameClient = reader.GetString(2),
                                    dateCreationClient = reader.GetDateTime(3),
                                    sign = reader.GetString(4),
                                    status = reader.GetBoolean(5),
                                    typeBusinessId = reader.GetInt32(6),
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
        public List<HeadQuartersModel> GetHeadquarters(int clientId, int idLocality)
        {
            var Headquarter = new List<HeadQuartersModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    if (clientId == 0 || idLocality == 0)
                    {
                        query = "SELECT * FROM \"headquarter\" INNER JOIN \"client\" ON \"headquarter\".\"clientId\" = \"client\".id INNER JOIN \"locality\" ON \"headquarter\".\"localityId\" = \"locality\".id";
                    }
                    else
                    {
                        query = "SELECT * FROM \"headquarter\" INNER JOIN \"client\" ON \"headquarter\".\"clientId\" = \"client\".id INNER JOIN \"locality\" ON \"headquarter\".\"localityId\" = \"locality\".id WHERE \"clientId\" = @clientId AND \"localityId\" = @localityId";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (clientId != 0 && idLocality != 0)
                        {
                            cmd.Parameters.AddWithValue("@clientId", clientId);
                            cmd.Parameters.AddWithValue("@localityId", idLocality);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) && !reader.IsDBNull(3) && !reader.IsDBNull(4) && !reader.IsDBNull(5) && !reader.IsDBNull(6) && !reader.IsDBNull(7) && !reader.IsDBNull(8))
                                {
                                    var headquarter = new HeadQuartersModel
                                    {
                                        id = reader.GetInt32(0),
                                        nameHeadquarter = reader.GetString(1),
                                        numberPhone = reader.GetString(2),
                                        address = reader.GetString(3),
                                        dateCreationHeadquarter = reader.GetDateTime(4),
                                        status = reader.GetBoolean(5),
                                        clientId = reader.GetInt32(6),
                                        localityId = reader.GetInt32(7),

                                        clientData = new ClientModel

                                        {
                                            id = reader.GetInt32(8),
                                            nitCc = reader.GetString(9),
                                            nameClient = reader.GetString(10),
                                            dateCreationClient = reader.GetDateTime(11),
                                            sign = reader.GetString(12),
                                            status = reader.GetBoolean(13),
                                            typeBusinessId = reader.GetInt32(14),
                                        },

                                        localitiesData = new LocalitiesModel
                                        {
                                            id = reader.GetInt32(15),
                                            nameLocality = reader.GetString(16),
                                            status = reader.GetBoolean(17),
                                            municipalityId = reader.GetInt32(18),
                                        }

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
                return new List<HeadQuartersModel>();
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


                    query = "SELECT * FROM \"headquarter\" WHERE id = @id";


                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", IdHeadQuarter);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) && !reader.IsDBNull(3) && !reader.IsDBNull(4) && !reader.IsDBNull(5) && !reader.IsDBNull(6))
                                {
                                    var headquarter = new HeadQuartersModelCreation
                                    {
                                        nameHeadquarter = reader.GetString(1),
                                        numberPhone = reader.GetString(2),
                                        address = reader.GetString(3),
                                        dateCreationHeadquarter = reader.GetDateTime(4),
                                        status = reader.GetBoolean(5),
                                        clientId = reader.GetInt32(6),
                                        localityId = reader.GetInt32(7),
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
                INSERT INTO ""headquarter"" (""nameHeadquarter"", ""numberPhone"", ""address"", ""dateCreationHeadquarter"", ""status"", ""clientId"", ""localityId"")
                VALUES (@nameHeadquarter, @numberPhone, @address, @dateCreationHeadquarter, @status, @clientId, @localityId)
                RETURNING *";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nameHeadquarter", HeadQuartersModel.nameHeadquarter);
                        cmd.Parameters.AddWithValue("@numberPhone", HeadQuartersModel.numberPhone);
                        cmd.Parameters.AddWithValue("@address", HeadQuartersModel.address);
                        cmd.Parameters.AddWithValue("@dateCreationHeadquarter", HeadQuartersModel.dateCreationHeadquarter);
                        cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Bit).Value = HeadQuartersModel.status ? "1" : "0";
                        cmd.Parameters.AddWithValue("@clientId", HeadQuartersModel.clientId);
                        cmd.Parameters.AddWithValue("@localityId", HeadQuartersModel.localityId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var headquarter = new HeadQuartersModelCreation
                                {
                                    nameHeadquarter = reader.GetString(1),
                                    numberPhone = reader.GetString(2),
                                    address = reader.GetString(3),
                                    dateCreationHeadquarter = reader.GetDateTime(4),
                                    status = reader.GetBoolean(5),
                                    clientId = reader.GetInt32(6),
                                    localityId = reader.GetInt32(7),

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
                    ""localityId"" = @localityId
                WHERE 
                    ""id"" = @id
                RETURNING *";

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

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var headQuarter = new HeadQuartersModelCreation
                                {
                                    nameHeadquarter = reader.GetString(1),
                                    numberPhone = reader.GetString(2),
                                    address = reader.GetString(3),
                                    dateCreationHeadquarter = reader.GetDateTime(4),
                                    status = reader.GetBoolean(5),
                                    clientId = reader.GetInt32(6),
                                    localityId = reader.GetInt32(7),
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

        #endregion
    }
}
