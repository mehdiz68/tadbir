$(document).ready(function () {

    /* Change Of Internal Link Selector */
    $("#TypeId").change(function () {
        var typeid = $(this).val();
        if ($(this).val()) {
            $("#SearchContainer").slideDown();
            $(".loading").show();
            $.ajax({
                ascync: false,
                type: "POST",
                url: "/Sliders/GetContent",
                data: '{TypeId:"' + typeid + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {
                        var firstitemText = $("#LinkId option:eq(0)").text();
                        $("#LinkId").html("");
                        $("#LinkId").append("<option value='0'>" + firstitemText + "</option>");
                        for (var i = 0; i < response.data.length; i++) {
                            $("#LinkId").append("<option value='" + response.data[i].Value + "'>" + response.data[i].Text + "</option>");
                        }

                    } else {
                        alert(response.Message);
                    }
                    $(".loading").hide();
                }
            });
        }
        else {
            $("#SearchContainer").slideUp();

        }
    });

    /*  Search In Internal Content */
    $('#SearchContent').keyup(function (e) {
        clearTimeout($.data(this, 'timer'));
        if (e.keyCode == 13)
            search(true);
        else
            $(this).data('timer', setTimeout(search, 680));
    });
    function search(force) {
        $(".loading").show();
        var existingString = $("#SearchContent").val();
        if (!force && existingString.length < 2) {
            $(".loading").hide();
            return; //wasn't enter, not > 1 char
        }
        $.get('/Sliders/SearchContent/?TypeId=' + $("#TypeId option:selected").val() + '&Keyword=' + existingString, function (response) {
            if (response.statusCode == 400) {
                var firstitemText = $("#LinkId option:eq(0)").text();
                $("#LinkId").html("");
                $("#LinkId").append("<option value='0'>" + firstitemText + "</option>");
                for (var i = 0; i < response.data.length; i++) {
                    $("#LinkId").append("<option value='" + response.data[i].Value + "'>" + response.data[i].Text + "</option>");
                }

                $("#LinkId option:eq(1)").prop('selected', true);
            } else {
                alert(response.Message);
            }

            $(".loading").hide();
        });
    }
});


function changeTitle() {
    $(".ajax-form-manager").attr("data-ajax-success", "$('.loading').hide();createMode('',3,1);changeTitle();");

    $("#SelectAttachement #OtherImageFieldset legend").text("تصاویر");

    $("#LinkContainer").remove();
    $("#SelectAttachement #OtherImageFieldset").prepend("<div id='LinkContainer'><br/> <p>لینک: <br/><input class='form-control' placeholder='لینک' style='text-alig:left;direction:ltr' type='text' id='txtLinkToJanebi' /></p></div>");

    $("#SelectAttachement #BtnAddToJanebi").val("انتخاب");

    var selectedAttachement = "";
    var IsImage = false;
    $("#FilesList").on("click", ".col-lg-3", function () {
        if ($(this).find("img").length > 0) {
            IsImage = true;
            selectedAttachement = $(this);
        }
    });
    //$("#BtnAddToJanebi").click(function () {
    //    if (!$("#txtLinkToJanebi").val()) {
    //        $("#txtLinkToJanebi").val("#");
    //    }
    //    if (IsImage) {
    //        //Get Selected Div
    //        var ID = $(selectedAttachement).attr("data-id");

    //        $.ajax({
    //            crossDomain: true,
    //            type: "POST",
    //            url: "http://" + window.location.host + "/Admin/Sliders/AddImageLink",
    //            data: '{ID:"' + ID + '",Link:"' + $("#txtLinkToJanebi").val() + '"}',
    //            contentType: "application/json; charset=utf-8",
    //            dataType: "json",
    //            success: function (response) {
    //            }
    //        });
    //    }

    //});
}