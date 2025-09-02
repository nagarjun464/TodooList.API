window.exporttabletoexcel =
{
    assemblyname: "Blazored.exportTableToExcel",

    exportTableToExcel: function (tableID, filename = '')
    {
        var downloadLink;
        var dataType = 'application/vnd.ms-excel';
        var tableSelect = document.getElementById(tableID);
        var tableHTML = tableSelect.outerHTML.replace(/ /g, '%20');

        // Specify file name
        filename = filename ? filename + '.xls' : 'excel_data.xls';

        // Create download link element
        downloadLink = document.createElement("a");

        document.body.appendChild(downloadLink);

        if (navigator.msSaveOrOpenBlob) {
            var blob = new Blob(['\ufeff', tableHTML], {
                type: dataType
            });
            navigator.msSaveOrOpenBlob(blob, filename);
        } else {
            // Create a link to the file
            downloadLink.href = 'data:' + dataType + ', ' + tableHTML;

            // Setting the file name
            downloadLink.download = filename;

            //triggering the function
            downloadLink.click();
        }
    },
    exportTable: function (tableID, filename = '') {

        $(document).find("#" + tableID).DataTable({
            "responsive": true, "lengthChange": true, "autoWidth": true, "paging": false, "ordering": false, "info": false, title: 'Data export',
            "buttons": [{
                extend: 'excelHtml5',
                title: filename
            },
            {
                extend: 'pdfHtml5',
                title: filename
            }]
        }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');



    }
}