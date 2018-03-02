<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchForm.aspx.cs" Inherits="SearchEngine.SearchForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title> DOODA</title>
  <link rel="Shortcut Icon" href="worm.png" type="image/x-icon" />


    <link href="bootstrap.min.css" rel="stylesheet" />
    <link href="font-awesome.min.css" rel="stylesheet" />
    <link href="responsive.css" rel="stylesheet" />
    <link href="style.css" rel="stylesheet" />
    <link href="StyleSheet1.css" rel="stylesheet" />
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<link rel="stylesheet" type="text/css" href="assets/css/bootstrap.min.css" media="screen">
<link rel="stylesheet" type="text/css" href="assets/css/font-awesome.min.css">
<link rel="stylesheet" type="text/css" href="assets/css/style.css" media="screen">
<link rel="stylesheet" type="text/css" href="assets/css/responsive.css" media="screen">
</head>
<body>


    <form id="form1" runat="server">
    <div class ="container" >
    
        <asp:Image  ID="Image1" runat="server" ImageAlign="Middle" ImageUrl="~/doda.gif" />
        <br />
        <h1 style = "color=#123456"> DOOOOOOODA</h1>
         <h5> search Engine </h5>
        <br />
    
        <asp:TextBox  ID="TextBox1" runat="server" Height="30px" Width="486px" OnTextChanged="TextBox1_TextChanged" AutoPostBack ="true"></asp:TextBox>
    
        <br />
        <asp:ListBox ID="ListBox1" runat="server" OnSelectedIndexChanged="ListBox1_SelectedIndexChanged1" Width="486px"></asp:ListBox>
    
        <br />
    
        <asp:CheckBox ID="ExactSearch" runat="server" CssClass="text-justify" OnCheckedChanged="CheckBox1_CheckedChanged" Text="Exact ?" />
    
        <asp:CheckBox ID="CheckBox1" runat="server" OnCheckedChanged="CheckBox1_CheckedChanged1" Text="checkSpelling" />
        <asp:CheckBox ID="CheckBox2" runat="server" Text="soundex" />
    
        <asp:Button  ID="Button1" runat="server" OnClick="Button1_Click" Text="Search" />
    
        <br />
    
    </div>
    <div class ="container">
          <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
    </div>
    </form>
    </body>
</html>
