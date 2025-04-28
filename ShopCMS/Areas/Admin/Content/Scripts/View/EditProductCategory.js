$(document).ready(function () {

    //Check Category Selection
    $(".btn-info").click(function () {
        var SelectedCategory = $(".CategoryManager select").last();
        if (SelectedCategory.find("option:selected").val() == 0) {
            $("#ErrorSummary").html("");
            $("#ErrorSummary").before("<p class='alert alert-error catmsg'>گروه و زیرگروه را انتخاب نمایید است! </p>").fadeIn(300);;
            $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
           
        }
        else {

            $(".loading").show();
            $.ajax({
                type: "POST",
                url: "/Products/EditCategory",
                data: '{CatId:"' + SelectedCategory.find("option:selected").val() + '",ProductId:"' + $("#ProductId").text() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        $("#ErrorSummary").html("");
                        $("#ErrorSummary").before("<p class='alert alert-success catmsg'>ویرایش شد! </p>").fadeIn(300);;
                        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
                        window.location.href = "/Admin/Products";
                    }
                    else {
                        $("#ErrorSummary").html("");
                        $("#ErrorSummary").before("<p class='alert alert-error catmsg'>خطایی رخ داد! </p>").fadeIn(300);;
                        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
                    }

                    $(".loading").hide(); 
                }
            });

        }

    });

});

