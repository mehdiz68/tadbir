$(document).ready(function () {
    $("#Add-Inner-State").click(function () {
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/SendWays/AddInnerState",
            data: '{id:' + $("#CurrentStateId").val() + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $(".Current-Inner-State-List").append("<p><span>" + response.name + "</span><span title='حذف' style='cursor:pointer;color:red' data-id=" + response.id + " class='glyphicon glyphicon-remove'></span></p>");
                }
                else
                    alert(response.Message);

                $(".loading").hide();

            },
            error: function (e) {
                alert(e);
                $(".loading").hide();

            }
        });
    });

    $(".form-horizontal").on("click",".glyphicon-remove",function () {
        $(".loading").show();
        var $this = $(this);
        $.ajax({
            type: "POST",
            url: "/SendWays/DeleteInnerState",
            data: '{id:' + $($this).attr("data-id") + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $this.parent().remove();
                }
                else
                    alert(response.Message);

                $(".loading").hide();

            },
            error: function (e) {
                alert(e);
                $(".loading").hide();

            }
        });
    });

    $("#Edit-City").click(function () {
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/SendWays/EditCurrentCity",
            data: '{Name:"' + $("#Current-City").val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {

                    alert(response.Message);
                }
                else
                    alert(response.Message);

                $(".loading").hide();

            },
            error: function (e) {
                alert(e);
                $(".loading").hide();

            }
        });
    });

    
    $("#Edit-State").click(function () {
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/SendWays/EditCurrentState",
            data: '{StateId:"' + $("#StateId").val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {

                    alert(response.Message);
                }
                else
                    alert(response.Message);

                $(".loading").hide();

            },
            error: function (e) {
                alert(e);
                $(".loading").hide();

            }
        });
    });
});