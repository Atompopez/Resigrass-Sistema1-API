<%@ Page Title="Menú" Language="C#" Async="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="ServerResigrassEstesies._Menu" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <section class="row" aria-labelledby="menuTitle">
            <style>
                body {
                    background-image: url('<%= ResolveUrl("~/Images/FondoLoginAceiteUsado.jpg") %>');
                    background-size: cover;
                    background-position: center;
                    background-repeat: no-repeat;
                    background-attachment: fixed;
                    opacity: 5.0;
                }
            </style>
            <div class="main-content">
                <h1 class="styled-title">Bienvenido al Menú</h1>
                <p class="intro-text">SISTEMA ADMINISTRADOR RESIGRASS.</p>
            </div>
        </section>

    </main>
</asp:Content>
