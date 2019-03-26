Imports System.Web.Services
Imports System.ComponentModel
Imports System.Xml
Imports System.Xml.Serialization


' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. ' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://localhost:2068/BikeRental.asmx")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class BikeRental

    Inherits System.Web.Services.WebService
    Private oRental As New ClsRental
    Private oRentalType As New ClsRentalType

    Private ReadOnly appSetting As New My.MySettings

    'Reads promotion min & max # items : format [3|5] represents from 3 to 5 rentals 
    Private ReadOnly strPromoSetting As String = appSetting.promosetting


#Region "RentalType"

    <WebMethod(Description:="for client to modify any rental price")>
    Public Function RentaltypeChangePrice(ByVal id As Integer, ByVal price As Double) As XmlDocument

        Try
            Dim Result As DataSet
            Dim Xmlout As New XmlDocument

            oRentalType.ID = id
            oRentalType.PRICE = price
            'updates the rentaltype in databse
            Result = oRentalType.ChangeRentalType()
            'creates xml document of rentaltype
            Xmlout.LoadXml(RentalTypeOutput(Result))

            RentaltypeChangePrice = Xmlout

        Catch ex As Exception
            Dim sExXml As String = "<rentaltype><error>" & ex.Message & "</error></rentaltype>"
            Dim ExXml As New XmlDocument
            ExXml.LoadXml(sExXml)
            RentaltypeChangePrice = ExXml
        End Try

    End Function

    <WebMethod(Description:="for client to modify promotion discount")>
    Public Function PromotionChangeDiscount('ByVal id As Integer,_ [enable this field if new promotions needed]
                                           ByVal discount As Double) As XmlDocument

        Try
            Dim Result As DataSet
            Dim Xmlout As New XmlDocument

            oRentalType.ID = 4 'id [enable this field if new promotions needed]
            oRentalType.DISCOUNT = discount
            'updates the rentaltype in databse
            Result = oRentalType.ChangeRentalType()
            'creates xml document of rentaltype
            Xmlout.LoadXml(RentalTypeOutput(Result))

            PromotionChangeDiscount = Xmlout

        Catch ex As Exception
            Dim sExXml As String = "<rentaltype><error>" & ex.Message & "</error></rentaltype>"
            Dim ExXml As New XmlDocument
            ExXml.LoadXml(sExXml)
            PromotionChangeDiscount = ExXml
        End Try
    End Function

    <WebMethod(Description:="to register a bike rent")>
    Public Function AddBikeRent(ByVal typeid As Integer, <XmlElement(GetType(MethodParams.RentItem()), IsNullable:=False)> ByVal rentitems As MethodParams.RentItem()) As XmlDocument


        Try

            Dim Xmlout As New XmlDocument
            Dim id As Integer

            'type must be any of [1-hour,2-day,3-week,4-promotion]
            If typeid < 0 Or typeid > 4 Then
                Throw New Exception("invalid rental mode value.")
            End If

            'at least an item must be provided
            If typeid < 4 And rentitems.Length = 0 Then
                Throw New Exception("missing rental item.")
            End If

            If typeid = 4 And (rentitems.Length < strPromoSetting.Split("|")(0) Or rentitems.Length > strPromoSetting.Split("|")(1)) Then
                Throw New Exception("promotion requires " & strPromoSetting & " rent items.")
            End If

            id = oRental.NewRental(typeid)

            For Each rentitem In rentitems
                Call oRental.AddItem(id, rentitem.typeid, rentitem.quantity, rentitem.start)
            Next

            'outputs rental details
            AddBikeRent = BikeRentDetail(id)

        Catch ex As Exception
            Dim sExXml As String = "<rental><error>" & ex.Message & "</error></rental>"
            Dim ExXml As New XmlDocument
            ExXml.LoadXml(sExXml)
            AddBikeRent = ExXml
        End Try

    End Function

    Private Function RentalTypeOutput(ByRef Rs As DataSet) As String

        Dim sXml As String = ""

        Try

            If Rs.Tables(0).Rows.Count > 0 Then
                sXml = "<rentaltype>"
                sXml &= "<id>" & Rs.Tables(0).Rows(0).Item("id").ToString() & "</id>"
                sXml &= "<typename>" & Rs.Tables(0).Rows(0).Item("typename").ToString() & "</typename>"
                sXml &= "<price>" & IIf(IsDBNull(Rs.Tables(0).Rows(0).Item("price")), "-", Rs.Tables(0).Rows(0).Item("price")) & "</price>"
                sXml &= "<discount>" & IIf(IsDBNull(Rs.Tables(0).Rows(0).Item("discount")), "-", Rs.Tables(0).Rows(0).Item("discount")) & "</discount>"
                sXml &= "</rentaltype>"
            Else
                sXml = "<rentaltype>"
                sXml &= "<error>unable to modify rentaltype</error>"
                sXml &= "</rentaltype>"
            End If

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        RentalTypeOutput = sXml

    End Function

#End Region

#Region "Rental"
    <WebMethod(Description:="to display a given bike rent detail")>
    Public Function BikeRentDetail(ByVal id As Integer) As XmlDocument

        Try
            Dim Result As DataSet
            Dim Xmlout As New XmlDocument
            Dim sXml As String = ""

            oRental.ID = id

            Result = oRental.RentalDetails

            If Result.Tables(0).Rows.Count > 0 Then
                sXml = "<rental>"
                sXml &= "<id>" & Result.Tables(0).Rows(0).Item("rentalid").ToString() & "</id>"
                sXml &= "<timestamp>" & Result.Tables(0).Rows(0).Item("timestamp").ToString() & "</timestamp>"
                sXml &= "<discount>" & Result.Tables(0).Rows(0).Item("discount").ToString() & "%</discount>"
                sXml &= "<totalprice>" & Result.Tables(0).Rows(0).Item("totalprice").ToString() & "</totalprice>"
                sXml &= "<items>"
                For i = 0 To Result.Tables(0).Rows.Count - 1
                    sXml &= "<id>" & Result.Tables(0).Rows(i).Item("rentalitem").ToString() & "</id>"
                    sXml &= "<type>" & Result.Tables(0).Rows(i).Item("typename").ToString() & "</type>"
                    sXml &= "<qty>" & Result.Tables(0).Rows(i).Item("quantity").ToString() & "</qty>"
                    sXml &= "<price>" & Result.Tables(0).Rows(i).Item("price").ToString() & "</price>"
                    sXml &= "<itemtotal>" & Result.Tables(0).Rows(i).Item("itemtotal").ToString() & "</itemtotal>"
                    sXml &= "<starting>" & Result.Tables(0).Rows(i).Item("pickup").ToString() & "</starting>"
                    sXml &= "<ending>" & Result.Tables(0).Rows(i).Item("dropoff").ToString() & "</ending>"
                Next


                sXml &= "</items>"
                sXml &= "</rental>"
            Else
                sXml = "<rentaltype>"
                sXml &= "<error>unable to list rental</error>"
                sXml &= "</rentaltype>"
            End If

            Xmlout.LoadXml(sXml)

            BikeRentDetail = Xmlout

        Catch ex As Exception
            Dim sExXml As String = "<rental><error>" & ex.Message & "</error></rental>"
            Dim ExXml As New XmlDocument
            ExXml.LoadXml(sExXml)
            BikeRentDetail = ExXml
        End Try


    End Function

#End Region

    Protected Overrides Sub Finalize()
        oRental = Nothing
        MyBase.Finalize()
    End Sub
End Class