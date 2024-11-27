<%@ Page Title="Administración de Sedes" Language="C#" Async="true" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Headquarters.aspx.cs" Inherits="ServerResigrassEstesies._headquarters" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <!-- Fondo con estilo moderno -->
        <style>
            body {
                background-image: url('<%= ResolveUrl("./Images/FondoLoginAceiteUsado.jpg") %>');
                background-size: cover;
                background-position: center;
                background-repeat: no-repeat;
                background-attachment: fixed;
                opacity: 5.0;
            }

            .styled-title {
                text-align: center;
                font-size: 2em;
                margin-top: 20px;
                color: #333;
            }

            .form-section {
                position: absolute; /* Mueve el cuadro respecto al contenedor */
                top: 0; /* Alineado al borde superior */
                left: 250px; /* Ajusta el valor para que coincida con el ancho de tu barra lateral */
                background: rgba(255, 255, 255, 0.9);
                padding: 20px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.2);
                height: 100vh; /* Altura completa */
                width: calc(100% - 250px); /* Usa todo el espacio restante a la derecha */
                box-sizing: border-box; /* Incluye padding y bordes en el ancho/alto */
            }





            .btn {
                margin-right: 10px;
            }
        </style>

        <!-- Contenedor Principal -->
        <section class="form-section">
            <h1 class="styled-title">Administración de Sedes</h1>
            <p class="text-center">Sistema Administrador Resigrass</p>

            <!-- Formulario -->
            <div class="container mt-4">
                <!-- Encabezado -->
                <h3 class="text-center mb-4" style="font-weight: 300; color: #333;">Gestión de Sedes</h3>

                <!-- Entrada de datos -->
                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="tbNombre" class="form-label" style="font-weight: 500;">Nombre de la Sede:</label>
                        <asp:TextBox ID="tbNombre" runat="server" CssClass="form-control" placeholder="Ingrese el nombre"></asp:TextBox>
                        <asp:Button ID="btnConsultar" runat="server" CssClass="btn btn-primary me-2" Text="Consultar" OnClick="btnConsultar_Click" />
                        <asp:Button ID="btnAgregar" runat="server" CssClass="btn btn-secondary" Text="Agregar" OnClick="btnAgregar_Click" />
                    </div>
                    <div class="col-md-6 d-flex align-items-end">
                    </div>
                </div>

                <!-- Tabla -->
                <div class="table-responsive">
                    <asp:GridView ID="gvSedes" runat="server" AutoGenerateColumns="False" CssClass="table custom-table">
                        <Columns>
                            <asp:BoundField DataField="Id" Visible="false" HeaderText="ID" />
                            <asp:BoundField DataField="clientId"  HeaderText="cliente prueba" />
                            <asp:BoundField DataField="nameHeadquarter" HeaderText="Nombre" />
                            <asp:BoundField DataField="address" HeaderText="Dirección" />
                            <asp:BoundField DataField="dateCreationHeadquarter" HeaderText="Fecha de creación" />
                            <asp:BoundField DataField="StatusText" HeaderText="Estado" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <style>
                body {
                    font-family: 'Inter', sans-serif;
                    background-color: #f9f9f9;
                    color: #333;
                }

                h3 {
                    font-size: 24px;
                }

                .form-control {
                    border-radius: 8px;
                    border: 1px solid #ddd;
                    font-size: 14px;
                }

                .btn {
                    border-radius: 8px;
                    padding: 8px 16px;
                    font-size: 14px;
                }

                .btn-primary {
                    background-image: linear-gradient(200deg,#ffb300,#ffc107,#ffca28,#ffd54f,#ffe082);
                    border: none;
                }

                .btn-secondary {
                    background-image: linear-gradient(200deg,#ffb300,#ffc107,#ffca28,#ffd54f,#ffe082);
                    border: none;
                }

                .btn:hover {
                    opacity: 0.9;
                }

                .custom-table {
                    width: 100%;
                    margin-top: 16px;
                    border-collapse: collapse;
                    font-size: 14px;
                }

                    .custom-table thead {
                        background-color: #f4f4f4;
                        color: #555;
                    }

                    .custom-table th,
                    .custom-table td {
                        padding: 12px 16px;
                        text-align: left;
                        border-bottom: 1px solid #eee;
                    }

                    .custom-table tbody tr:hover {
                        background-color: #f1f5f8;
                    }

                    .custom-table th {
                        font-weight: 500;
                    }

                    .custom-table td {
                        font-weight: 300;
                        color: #666;
                    }

                .table-responsive {
                    border-radius: 8px;
                    overflow: hidden;
                }
            </style>


        </section>
    </main>
</asp:Content>
