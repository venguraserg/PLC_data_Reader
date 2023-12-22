using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSheet
{
    internal class GoogleDataSender
    {
        readonly string[] Scopes = { SheetsService.Scope.Spreadsheets }; /*Для доступа только к таблицам*/
        readonly string ApplicationName = "GoogleSheets"; /*Название приложения*/
        readonly string SpreadsheetId;/*Идентификатор таблицы*/
        readonly string sheet; /*Название листа с которым работаем*/
        readonly SheetsService service;
        public GoogleDataSender(string SpreadsheetId, string sheet)
        {
            this.SpreadsheetId = SpreadsheetId;
            this.sheet = sheet;

            GoogleCredential credential; /*Получаем доступ к учётным данным*/
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
        public void CreateEntry(List<object> rowData) /*Метод добавления строки в таблицу*/
        {
            var range = $"{this.sheet}!A:Z";
            var valueRange = new ValueRange();
            List<object>? objectList = rowData;


            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();

        }
    }
}