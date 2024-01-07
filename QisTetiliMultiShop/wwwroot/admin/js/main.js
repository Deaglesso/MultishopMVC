const ButtonList = document.querySelectorAll(".sweet-delete-custom");

ButtonList.forEach(btn => {
    btn.addEventListener("click", function (e) {
        e.preventDefault();
        var endpoint = btn.getAttribute("href");
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(endpoint).
                    then(response => response.json()).
                    then(data => {
                        if (data.status == 200) {
                            Swal.fire({
                                title: "Deleted!",
                                text: "Your file has been deleted.",
                                icon: "success"
                            }).then(function () {
                                window.location.reload();
                            });

                        }
                        else
                        {
                            Swal.fire({
                                title: "Failed!",
                                text: "Your file has not been deleted.",
                                icon: "error"
                            });
                        }
                    })
                
            }
        });
    })
})