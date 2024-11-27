<%@ Page Title="Login Page" Language="C#" Async="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ServerResigrassEstesies._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <section class="row" aria-labelledby="loginTitle">
            <h1 id="loginTitle" class="centered-title" >
                <img src="./Images/logoResigrass.png" alt="Logo de Resigrass" class="logo-image" />
            </h1>

     <style>
        body {
            background-image: url('<%= ResolveUrl("./Images/FondoLoginAceiteUsado.jpg") %>');
            background-size: cover;
            background-position: center;
            background-repeat: no-repeat;
            background-attachment: fixed;
            opacity:5.0;
            
            } 

    </style>



            <!-- Formulario de Login -->
            <div class="login-form">
                <asp:Label ID="lblUsername" runat="server" Text="Usuario:" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Ingrese su usuario"></asp:TextBox>

                <asp:Label ID="lblPassword" runat="server" Text="Contraseña:" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Ingrese su contraseña"></asp:TextBox>

                <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>
                <asp:Button ID="btnLogin" Text="INGRESAR" CssClass="btn btn-primary btn-lg custom-button" OnClick="btnLogin_Click" runat="server"  />
            </div>


        </section>
    </main>
</asp:Content>
