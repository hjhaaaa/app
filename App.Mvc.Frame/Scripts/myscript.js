
$(document).ready(function () {
    $("#bt11").click(
        function () {
            $.ajax({
                type: "get",
                url: "http://www.taohuadao.club/api/app/logo",
                dataType: "json",
                data: "",
                contentType: 'application/json;charset=utf-8',//向后台传送格式
                success: function (data) {
                    $("#h11").html(data);
                },
                error: function (jqXHR) {
                    alert("发生错误a：" + jqXHR.status + jqXHR.responseText);
                }

            })
        }
    )


})

