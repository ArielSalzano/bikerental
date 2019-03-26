Imports System.Data
Imports System.Data.SqlClient

Public Class ClsRental

#Region "local"
    Private mID As Integer
    'Private mTYPENAME As String
    Private mPRICE As Single
    Private mDISCOUNT As Integer

#End Region

#Region "access"
    Property ID() As Integer
        Get
            Return mID
        End Get
        Set(ByVal value As Integer)
            mID = value
        End Set
    End Property

    'Property TYPENAME() As String
    'Get
    'Return mTYPENAME
    'End Get
    'Set(ByVal value As String)
    '       mTYPENAME = value
    'End Set
    'End Property

    Property PRICE() As Single
        Get
            Return mPRICE
        End Get
        Set(ByVal value As Single)
            mPRICE = value
        End Set
    End Property
    Property DISCOUNT() As Integer
        Get
            Return mDISCOUNT
        End Get
        Set(ByVal value As Integer)
            mDISCOUNT = value
        End Set
    End Property



#End Region

#Region "methods"

    Public Function RentalDetails() As DataSet
        Try
            Dim Cmd As New SqlCommand()
            Dim DB As New ClsDB()
            Dim Resultset As DataSet

            Cmd.CommandType = CommandType.StoredProcedure
            Cmd.CommandText = "rentaldetail"

            Cmd.Parameters.Add(New SqlParameter("@id", SqlDbType.SmallInt))
            Cmd.Parameters("@id").IsNullable = False
            Cmd.Parameters("@Id").Value = ID

            Resultset = DB.Exec(Cmd)

            Return Resultset

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Function

    Public Function NewRental(ByVal typeid As Integer) As Integer

        Try
            Dim Cmd As New SqlCommand()
            Dim DB As New ClsDB()
            Dim Resultset As DataSet

            Cmd.CommandType = CommandType.StoredProcedure
            Cmd.CommandText = "rental_add"

            Cmd.Parameters.Add(New SqlParameter("@typeid", SqlDbType.SmallInt))
            Cmd.Parameters("@typeid").IsNullable = True
            Cmd.Parameters("@typeid").Value = typeid

            Resultset = DB.Exec(Cmd)

            If Resultset.Tables(0).Rows.Count > 0 Then

                Return Resultset.Tables(0).Rows(0).Item("id")
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try


    End Function

    Public Sub AddItem(ByVal rentalid As Integer, ByVal typeid As Integer, ByVal quantity As Integer, pickup As DateTime)

        Try
            Dim Cmd As New SqlCommand()
            Dim DB As New ClsDB()

            Cmd.CommandType = CommandType.StoredProcedure
            Cmd.CommandText = "rentalitem_add"

            Cmd.Parameters.Add(New SqlParameter("@rentalid", SqlDbType.BigInt))
            Cmd.Parameters("@rentalid").IsNullable = False
            Cmd.Parameters("@rentalid").Value = rentalid

            Cmd.Parameters.Add(New SqlParameter("@typeid", SqlDbType.SmallInt))
            Cmd.Parameters("@typeid").IsNullable = False
            Cmd.Parameters("@typeid").Value = typeid

            Cmd.Parameters.Add(New SqlParameter("@quantity", SqlDbType.TinyInt))
            Cmd.Parameters("@quantity").IsNullable = False
            Cmd.Parameters("@quantity").Value = quantity

            Cmd.Parameters.Add(New SqlParameter("@pickup", SqlDbType.DateTime))
            Cmd.Parameters("@pickup").IsNullable = False
            Cmd.Parameters("@pickup").Value = pickup

            Call DB.Exec(Cmd)

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try


    End Sub


#End Region

End Class
