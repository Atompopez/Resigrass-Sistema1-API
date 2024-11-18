using Npgsql;
using ResiGrass_API.Models;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using DocumentFormat.OpenXml.Office.Word;



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

                    using (var cmd = new NpgsqlCommand("SELECT * FROM municipality WHERE status != 0", conn))
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
        public List<CollectorModelInsert> InsertCollector(CollectorModelInsert collectorModel, loginCreationCollectorModel loginCollectorModel)
        {
            var collectors = new List<CollectorModelInsert>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    // Primero insertar en la tabla loginCollector
                    string queryLogin = @"
            INSERT INTO resigrass.""loginCollector"" (""user"", ""password"", ""status"")
            VALUES (@user, @password, @status)
            RETURNING id";

                    int loginCollectorId;

                    using (var cmdLogin = new NpgsqlCommand(queryLogin, conn))
                    {
                        // Hash de la contraseña antes de la inserción
                        string hashedPassword = HashPassword(loginCollectorModel.password);

                        cmdLogin.Parameters.AddWithValue("@user", loginCollectorModel.user);
                        cmdLogin.Parameters.AddWithValue("@password", hashedPassword); // Usar la contraseña hasheada                                                                                      
                        var statusBit = loginCollectorModel.status ? new BitArray(new[] { true }) : new BitArray(new[] { false });

                        // Asignar el valor del parámetro como un array de bits
                        cmdLogin.Parameters.AddWithValue("@status", statusBit);



                        // Obtener el id generado
                        loginCollectorId = (int)cmdLogin.ExecuteScalar();
                    }

                    // Ahora insertar en la tabla collector usando el loginCollectorId
                    string queryCollector = @"
            INSERT INTO resigrass.collector (""nameCollector"", ""numberPhoneCollector"", ""dateCreationCollector"", ""status"", ""loginCollectorId"", ""typeCollectorId"")
            VALUES (@nameCollector, @numberPhoneCollector, @dateCreationCollector, @status, @loginCollectorId, @typeCollectorId)
            RETURNING *";

                    using (var cmdCollector = new NpgsqlCommand(queryCollector, conn))
                    {
                        cmdCollector.Parameters.AddWithValue("@nameCollector", collectorModel.nameCollector);
                        cmdCollector.Parameters.AddWithValue("@numberPhoneCollector", collectorModel.numberPhoneCollector);
                        cmdCollector.Parameters.AddWithValue("@dateCreationCollector", DateTime.Now);
                        var statusBit = loginCollectorModel.status ? new BitArray(new[] { true }) : new BitArray(new[] { false });
                        cmdCollector.Parameters.AddWithValue("@status", statusBit);
                        cmdCollector.Parameters.AddWithValue("@loginCollectorId", loginCollectorId);
                        cmdCollector.Parameters.AddWithValue("@typeCollectorId", collectorModel.typeCollectorId);

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
                                    typeCollectorId = reader.GetInt32(6)
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

        // Función para hashear la contraseña
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
                               tc.""descriptionCollector""
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
                                        var collectorData = new CollectorsModelSelect
                                        {
                                            id = collectorReader.GetInt32(0),
                                            nameCollector = collectorReader.GetString(1),
                                            numberPhoneCollector = collectorReader.GetString(2),
                                            typeCollectorsModelId = new TypeCollectorsModelSelect
                                            {
                                                descriptionCollector = collectorReader.GetString(3),
                                            }
                                        };

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
                    conn.Open();
                    string query = @"
                INSERT INTO collection ( ""receivedDate"", ""endDate"", ""fullPayment"", ""priceUnit"", ""netWeight"",observations,""receivedFull"",""bowlEmpty"",""collectorId"",""headquarterId"",""measureId"",""methodPaymentId"",""productId"")
                VALUES ( @collectedName, @receivedDate, @endDate, @fullPayment, @priceUnit, @netWeight, @observations, @receivedFull, @bowlEmpty,  @collectorId, @headquarterId, @measureId, @methodPaymentId, @productId)
                RETURNING *";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@receivedDate", CollectionModel.receivedDate);
                        cmd.Parameters.AddWithValue("@endDate", DateTime.Now);
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

        #region Records
        public List<RecolectionModel> GetRecordsDueInTwoDays()
        {
            var records = new List<RecolectionModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("SELECT * FROM collection WHERE id = 1", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    var record = new RecolectionModel
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

                                    records.Add(record);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los registros: {ex.Message}");
                return new List<RecolectionModel>();
            }

            return records;
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
                return  "";
            }

            return "collectors";
        }



        #endregion

        #region CollectorLoginGet
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
//                            var storedStatus = reader.GetBoolean(reader.GetOrdinal("status"));

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

    }

}
