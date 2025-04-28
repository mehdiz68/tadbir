$(document).ready(function () {
    //---------------------------------
    //Sort
    $(".table").sortable({
        items: ".rowItem",
        cursor: 'move',
        opacity: 0.6,
        placeholder: "ui-state-highlight",
        update: function () {
            sendPageOrderToServer();
        }
    });
    function sendPageOrderToServer() {
        var order = $(".table").sortable("toArray");
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: "/Admin/ProductAttributes/SortItems",
            data: '{ids:"' + order + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $(".table-responsive").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> انجام شد . </p>").fadeIn(300);
                    $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                } else {
                    $(".table-responsive").before("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> خطا </p>").fadeIn(300);;
                    $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                }
            }
        });
    }

});