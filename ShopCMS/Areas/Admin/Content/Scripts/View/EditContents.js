$(document).ready(function () {
    //------------------------------------------------------------------------------
    $(".Newlegend").click(function () {
        $(this).parent().find(".NewFieldset").slideToggle();
    });
    $(".NewFieldset .text-left .glyphicon-remove").click(function () {
        $(this).parent().parent().slideToggle();
    });
    //------------------------------------------------------------------------------
    $("#IsDefault").change(function () {
        if ($(this).is(":checked")) {
            $("#IsAbout").prop("checked", false);
            $("#IsRegister").prop("checked", false);
        }
    });
    $("#IsAbout").change(function () {
        if ($(this).is(":checked")) {
            $("#IsDefault").prop("checked", false);
            $("#IsRegister").prop("checked", false);
        }
    });
    $("#IsRegister").change(function () {
        if ($(this).is(":checked")) {
            $("#IsAbout").prop("checked", false);
            $("#IsDefault").prop("checked", false);
        }
    });
    //------------------------------------------------------------------------------
    /* Source Manager */

    $("#AddSource").click(function () {
        var expression = /[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?/gi;
        var regex = new RegExp(expression);
        var source = $("#SourceName").val();
        var SourceLink = ($("#SourceLink").val() == "" ? "unknown" : $("#SourceLink").val());
        var validUrl = true;
        if (SourceLink != "unknown") {
            if (!SourceLink.match(regex)) {
                validUrl = false;
            }
        }
        if (validUrl) {
            var i = 1;
            var currentIndex = $('#Sources tr:last td:first').text();
            if (currentIndex != "")
                i = parseInt(currentIndex) + 1;
            if (source != "") {
                $("#Sources").append("<tr><td>" + i + " </td><td><input type='hidden' name='SourceLink' id='SourceLink-" + i + "'  value='" + SourceLink + "'/><a target='_blank' href='" + SourceLink + "'> <input type='hidden' name='SourceTitle' id='source-" + i + "' value='" + source + "' />" + source + "</a></td><td><span style='cursor:pointer' class='RemoveSource glyphicon glyphicon-remove' title='حذف'></span></td></tr>");
                $("#SourceName").css("border-color", "#d2d6de");
                $("#SourceLink").css("border-color", "#d2d6de");
                $("#SourceName").val("");
                $("#SourceLink").val("");

            }
            else {
                $("#SourceName").css("border-color", "#FF0000");
                swal({ title: "", text: "منبع را وارد نمایید", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                $(".loading").hide();
            }
        }
        else {
            $("#SourceLink").css("border-color", "#FF0000");
            swal({ title: "", text: "آدرس منبع معتبر نیست ! با http:// شروع شود", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
            $(".loading").hide();
        }
    });
    $("#Sources").on("click", ".RemoveSource", function () {
        var source = $(this).parent().prev().find("a").text();
        var $this = $(this);
        if (source != "") {
            $this.parent().parent().remove();
            $("#SourceName").css("border-color", "#d2d6de");
        }
        else {
            $("#SourceName").css("border-color", "#FF0000");
            swal({ title: "", text: "منبع را وارد نمایید", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
        }

    });

    //------------------------------------------------------------------------------
    /* Category Manager */
    $(".tree li a").css("background", "#fff").css("color", "#666").css("border", "1px solid #ccc");
    var CurrentCat = $(".tree li a[data-id='" + $("#SelectedCategoryId").val() + "']");
    CurrentCat.css("background", "#3c8dbc").css("color", "#ffffff").css("border", "1px solid #3c8dbc");
    CurrentCat.parent().find("ul li a").css("background", "#3c8dbc").css("color", "#ffffff").css("border", "1px solid #3c8dbc");
    $("#SelectedCategory").text(CurrentCat.text());

    $("#TreeCategoryfieldset .tree li").on("click", "a", function (e) {
        e.preventDefault();
        $(".tree li a").css("background", "#fff").css("color", "#666").css("border", "1px solid #ccc");
        $(this).css("background", "#3c8dbc").css("color", "#ffffff").css("border", "1px solid #3c8dbc");
        //$(this).parent().find("ul li a").css("background", "#3c8dbc").css("color", "#ffffff").css("border", "1px solid #3c8dbc");

        $("#SelectedCategoryId").val($(this).attr("data-id"));
        $("#SelectedCategory").text($(this).text());
    });

    $("#ContentTypeId").change(function () {
        $(".loading").show();
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: '/Admin/Contents/FilterCategory/',
            data: '{ContentTypeId:"' + $("#ContentTypeId option:selected").val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.success == 1) {
                    $("#TreeCategory").html(response.data);

                }

                $(".loading").hide();
            }
        });
    });

    //------------------------------------------------------------------------------
    /* Tag Manager */
    /* Search  */
    $('#SearchTag').keyup(function (e) {
        clearTimeout($.data(this, 'timer'));
        if (e.keyCode == 13)
            search(true);
        else
            $(this).data('timer', setTimeout(search, 680));
    });
    function search(force) {
        $(".loading").show();
        var existingString = $("#SearchTag").val();
        if (!force && existingString.length < 2) {
            $(".loading").hide();
            return; //wasn't enter, not > 1 char
        }
        $.get('/Contents/SearchTag/?TagName=' + existingString, function (data) {
            $("#TagContainer .Tagcontent").html("");
            for (var i = 0; i < data.data.length; i++) {
                $("#TagContainer .Tagcontent").append("<div class='col-lg-4 col-md-4 col-sm-6 col-xs-12' style='padding:0'><input data-id='" + data.data[i].Id + "' name='" + data.data[i].TagName + "' type='checkbox' value='true'><input name='" + data.data[i].TagName + "' type='hidden' value='false'><label style='font-weight:normal'>" + data.data[i].TagName + "</label></div>");
            }
            $("#TagContainer .Tagcontent").append("<div class='clearfix'></div>");

            $(".loading").hide();
        });
    }
    /* Show All */
    $("#ResetTag").click(function () {
        $.get('/Contents/AllTag/', function (data) {
            $("#TagContainer .Tagcontent").html("");
            for (var i = 0; i < data.data.length; i++) {
                $("#TagContainer .Tagcontent").append("<div class='col-lg-4 col-md-4 col-sm-6 col-xs-12' style='padding:0'><input data-id='" + data.data[i].Id + "' name='" + data.data[i].TagName + "' type='checkbox' value='true'><input name='" + data.data[i].TagName + "' type='hidden' value='false'><label style='font-weight:normal'>" + data.data[i].TagName + "</label></div>");
            }
            $("#TagContainer .Tagcontent").append("<div class='clearfix'></div>");
            $("#SearchTag").val("");
            $("#AddTagText").val("");
            //$("#SelectedTags").html("");
            $("#SelectedTagsForEdit").html("");
            $("#SelectedTags .RemoveTagSelection").each(function () {
                $(".Tagcontent input[data-id='" + $(this).attr("data-id") + "']").prop("checked", true);
            });
            $(".loading").hide();
        });

    });
    /*Select For Content*/
    $("#SelectForContent").click(function () {
        //$("#SelectedTagsForEdit").html("");
        var checkedInputs = $(".Tagcontent input:checked");
        if (checkedInputs.length > 0) {
            var ids = "";
            for (var i = 0; i < checkedInputs.length; i++) {
                if (!$("#SelectedTags input[value='" + checkedInputs.eq(i).attr("data-id") + "']").length)
                    $("#SelectedTags").append("<span><input type='hidden' name='TagId' id='Tag-" + checkedInputs.eq(i).attr("data-id") + "' value='" + checkedInputs.eq(i).attr("data-id") + "' /><span data-id='" + checkedInputs.eq(i).attr("data-id") + "' style='cursor:pointer' class='RemoveTagSelection glyphicon glyphicon-remove'></span> " + checkedInputs.eq(i).attr("name") + " </span>&nbsp;&nbsp;");
            }

        }
        else
            $("#SelectedTags").html("");
    });
    /*Deelte From Selection */
    $("#SelectedTags").on("click", ".RemoveTagSelection", function () {
        if (confirm("آیا مطمئن هستید؟")) {
            var Id = $(this).attr("data-id");
            var $this = $(this);
            $this.parent().remove();
            $(".Tagcontent input[data-id='" + Id + "']").prop("checked", false);
            if ($("#SelectedTags > span").size() == 0)
                $("#SelectedTags").html("");
        }
    });
    /*Select For Edit*/
    $("#EditTag").click(function () {
        $("#SelectedTags").html("");
        var checkedInputs = $(".Tagcontent input:checked");
        if (checkedInputs.length > 0) {

            var html = "<p><span id='closeEditTag' class='glyphicon glyphicon-remove' style='color:red;cursor:pointer'></span> برچسب های انتخابی برای ویرایش:  </p>";
            $("#SelectedTagsForEdit").html(html);
            html = "<div class='table-responsive'><table class='table'><tr><th>برچسب</th><th></th>";
            for (var i = 0; i < checkedInputs.length; i++) {
                html += "<tr><td><input type='text' placeholder='عنوان جدید را وارد نمایید...' value='" + checkedInputs.eq(i).attr("Name") + "'></td><td><span data-id='" + checkedInputs.eq(i).attr("data-id") + "' style='cursor:pointer' title='ویرایش' class='EditTag glyphicon glyphicon-edit'></span></td></tr>";
            }
            html += "</table></div>";
            $("#SelectedTagsForEdit").append(html);

            $("#closeEditTag").click(function () {
                $("#SelectedTagsForEdit").html("");
                $(".Tagcontent input").prop("checked", false);
            });

        }
        else
            $("#SelectedTagsForEdit").html("");
    });
    /* Edit Tag */
    $("#SelectedTagsForEdit").on("click", ".EditTag", function () {
        var text = $(this).parent().prev().find("input").val();
        var id = $(this).attr("data-id");
        $(".loading").show();
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: '/Contents/EditTags/',
            data: '{id:"' + id + '",tagName:"' + text + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    swal({ title: "", text: "ویرایش انجام شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }
                else {
                    swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }
                $(".loading").hide();
            }
        });
    });
    $("#DeleteTag").click(function () {
        if (confirm("آیا مطمئن هستید؟ تگ های انتخابی به طور کلی حذف خواهند شد")) {
            var checkedInputs = $(".Tagcontent input:checked");
            if (checkedInputs.length > 0) {
                var ids = "";
                for (var i = 0; i < checkedInputs.length; i++) {
                    ids += checkedInputs.eq(i).attr("data-id") + ",";
                }
                ids = ids.substring(0, ids.length - 1);
                var id = $(this).attr("data-id");
                $(".loading").show();
                $.ajax({
                    crossDomain: true,
                    type: "POST",
                    url: '/Contents/DeleteTags/',
                    data: '{ids:"' + ids + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.statusCode == 200) {
                            for (var i = 0; i < response.deletedTagId.length; i++) {
                                $(".Tagcontent input[data-id=" + response.deletedTagId[i] + "]").parent().remove();
                            }

                            $("#tag-Message").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> " + response.Message + " </p>").fadeIn(1000);
                            $(".catmsg").delay(8000).fadeOut(1000, function () { $(this).remove(); });
                        }
                        else {

                            $("#tag-Message").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> " + response.Message + " </p>").fadeIn(1000);
                            $(".catmsg").delay(8000).fadeOut(1000, function () { $(this).remove(); });
                        }
                        $(".loading").hide();
                    }
                });
            }
            else
                               swal({ title: "", text: "برچسبی انتخاب نشده است", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
        }
    });
    /*Add Tag*/
    $("#AddNewTag").click(function () {
        if ($("#AddTagText").val()) {
            
            var languageid = 1;
            if ($("#LanguageId option:selected").val())
                languageid = parseInt($("#LanguageId option:selected").val());
            else if ($("#LanguageId").val())
                languageid = parseInt($("#LanguageId").val());
            
            $(".loading").show();
            $.ajax({
                crossDomain: true,
                type: "POST",
                url: '/Contents/AddTags/',
                data: '{TagName:"' + $("#AddTagText").val() + '",LanguageId:"' + languageid + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        swal({ title: "", text: "ثبت شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        $("#AddTagText").val("");
                    }
                    else {
                        swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    }
                    $(".loading").hide();
                }
            });
        }
        else
            swal({ title: "", text: "نام برچسب را وارد نمایید", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
    });


    //Select Category Of Selected Tree
    $("#tree li a").each(function () {
        if ($(this).next().hasClass("subItem")) {
            $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: 0px;cursor:pointer'>   [+]   <span>");
            $(this).parent().find(".subItem").hide();
        }
    });
    $("#tree").on("click", ".PlusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='MinusTree' style='position: relative;bottom: 0px;cursor:pointer'>   [-]   <span>");
        $(this).parent().find(".subItem").first().show();
        $(this).parent().find(".PlusTree").first().remove();

    });
    $("#tree").on("click", ".MinusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: 0px;cursor:pointer'>   [+]   <span>");
        $(this).parent().find(".subItem").first().hide();
        $(this).parent().find(".MinusTree").first().remove();
    });
});