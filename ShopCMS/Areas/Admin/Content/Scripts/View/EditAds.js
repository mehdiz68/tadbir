$(document).ready(function () {

    /* Load Data */
    var typeid = $("#TypeId option:selected").val();
    if ($("#TypeId option:selected").val()) {
        $("#SearchContainer").slideDown();
        $(".loading").show();
        if ($("#CurrentLinkId").text() != "") {

            $.ajax({
                ascync: false,
                type: "POST",
                url: "/adveresting/GetContentByLinkId",
                data: '{TypeId:"' + typeid + '",LinkId:"' + $("#CurrentLinkId").text() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {
                        $("#LinkId").html("");
                        $("#LinkId").append("<option value='0'>--همه صفحات--</option>");
                        for (var i = 0; i < response.data.length; i++) {
                            if (response.data[i].Value == $("#CurrentLinkId").text())
                                $("#LinkId").append("<option selected='selected' value='" + response.data[i].Value + "'>" + response.data[i].Text + "</option>");
                            else
                                $("#LinkId").append("<option value='" + response.data[i].Value + "'>" + response.data[i].Text + "</option>");
                        }

                        $("#LinkId option:eq(1)").prop('selected', true);
                    } else {
                        alert(response.Message);
                    }
                    $(".loading").hide();
                }
            });
        }
        else {
            $.ajax({
                ascync: false,
                type: "POST",
                url: "/adveresting/GetContent",
                data: '{TypeId:"' + typeid + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {
                        $("#LinkId").html("");
                        $("#LinkId").append("<option value='0'>--همه صفحات--</option>");
                        for (var i = 0; i < response.data.length; i++) {
                            if (response.data[i].Value == $("#CurrentLinkId").text())
                                $("#LinkId").append("<option selected='selected' value='" + response.data[i].Value + "'>" + response.data[i].Text + "</option>");
                            else
                                $("#LinkId").append("<option value='" + response.data[i].Value + "'>" + response.data[i].Text + "</option>");
                        }

                        $("#LinkId option:eq(1)").prop('selected', true);
                    } else {
                        alert(response.Message);
                    }
                    $(".loading").hide();
                }
            });

        }
    }
    else {
        $("#SearchContainer").slideUp();

    }

    /* Change Of Internal Link Selector */
    $("#TypeId").change(function () {
        var typeid = $(this).val();
        if ($(this).val()) {
            $("#SearchContainer").slideDown();
            $(".loading").show();
            $.ajax({
                ascync: false,
                type: "POST",
                url: "/adveresting/GetContent",
                data: '{TypeId:"' + typeid + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {
                        $("#LinkId").html("");
                        $("#LinkId").append("<option value='0'>--همه صفحات--</option>");
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
        $.get('/adveresting/SearchContent/?TypeId=' + $("#TypeId option:selected").val() + '&Keyword=' + existingString, function (response) {
            if (response.statusCode == 400) {
                $("#LinkId").html("");
                $("#LinkId").append("<option value='0'>--انتخاب نشده--</option>");
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

