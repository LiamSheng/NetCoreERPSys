var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": '/admin/product/getallapi'
        },
        "columns": [
            { "data": 'title', "width": "15%" },
            { "data": 'isbn', "width": "15%" },
            { "data": 'listPrice', "width": "15%" },
            { "data": 'author', "width": "15%" },
            { "data": 'category.name', "width": "15%" },
            {
                "data": 'id',
                "width": "15%",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a onClick=Delete('/admin/product/delete?id=${data}') class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Delete
                            </a>
                        </div>
                    `;
                }
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