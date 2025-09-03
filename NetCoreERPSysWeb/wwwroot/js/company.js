var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "state", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                // ... in company.js ...
                "render": function (data) {
                    return `
                                <div class="w-75 btn-group" role="group">
                                    <a href="/Admin/Company/Upsert?id=${data}"
                                       class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>

                                    <a onClick=Delete('/Admin/Company/Delete?id=${data}')
                                       class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                                </div>
                            `
                },
                //...
                "width": "15%"
            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this imaginary file!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    // data 是 JSON 返回值 { success = true, message = "Deleted Successful!" }
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
            swal("Poof! Your imaginary file has been deleted!", {
                icon: "success",
            });
        } else {
            swal("Your imaginary file is safe!");
        }
    });
}