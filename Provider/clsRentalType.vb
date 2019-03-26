Imports System.Data
Imports System.Data.SqlClient

Public Class ClsRentalType

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

    Public Function ChangeRentalType() As DataSet
        Try
            Dim Cmd As New SqlCommand()
            Dim DB As New ClsDB()
            Dim Resultset As DataSet

            Cmd.CommandType = CommandType.StoredProcedure
            Cmd.CommandText = "rentaltype_save"

            Cmd.Parameters.Add(New SqlParameter("@id", SqlDbType.SmallInt))
            Cmd.Parameters("@id").IsNullable = True
            Cmd.Parameters("@Id").Value = ID

            Cmd.Parameters.Add(New SqlParameter("@price", SqlDbType.SmallMoney))
            Cmd.Parameters("@price").IsNullable = True
            If PRICE = 0 Then
                Cmd.Parameters("@price").Value = System.DBNull.Value
            Else
                Cmd.Parameters("@price").Value = PRICE
            End If

            Cmd.Parameters.Add(New SqlParameter("@discount", SqlDbType.TinyInt))
            Cmd.Parameters("@discount").IsNullable = True
            If DISCOUNT = 0 Then
                Cmd.Parameters("@discount").Value = System.DBNull.Value
            Else
                Cmd.Parameters("@discount").Value = DISCOUNT
            End If

            Resultset = DB.Exec(Cmd)

            Return Resultset

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Function




#End Region

End Class
