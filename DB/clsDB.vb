Imports System.Data.Sql
Imports System.Data.SqlClient


Public Class ClsDB

    Private ReadOnly dbConnString As String = ConfigurationManager.ConnectionStrings("DB").ConnectionString
    Private ReadOnly dbConnection As SqlConnection = New SqlConnection(dbConnString)

    Private dbCmdTimeout As Integer

    Public Sub SetTimeOut(ByVal timeout As Integer)
        dbCmdTimeout = timeout
    End Sub


    Public Function Exec(ByRef sqlCmd As SqlCommand) As DataSet

        Dim Dataset As New DataSet()
        Dim Adapter As New SqlDataAdapter()

        sqlCmd.Connection = dbConnection

        If Not IsNothing(dbCmdTimeout) Then sqlCmd.CommandTimeout = dbCmdTimeout
        Adapter.SelectCommand = sqlCmd

        'sets timeout to its default value
        SetTimeOut(30)

        Try
            Adapter.Fill(Dataset)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Return Dataset

    End Function




End Class