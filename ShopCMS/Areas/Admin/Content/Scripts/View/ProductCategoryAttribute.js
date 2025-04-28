$(document).ready(function () {
    showAttributes(1);

    //Select All Attribute
    $(".x_panel").on("click", "#checkAll", function () {
        if ($(this).is(":checked"))
            $("#CategoryAttributes input").prop("checked", true);
        else
            $("#CategoryAttributes input").prop("checked", false);
    });

    //Add Attribute To Category
    $(".x_panel").on("click", "#Add-attribute", function () {
        var $this = $(this);
        var attributes = [];
        $("#CategoryAttributes .List .NewFieldset").each(function () {
            var $this = $(this).find(".Newlegend input[type='checkbox']");
            if ($this.is(":checked")) {
                var obj = {
                    AttributeId: null,
                    GroupId: $this.parent().parent().parent().attr("data-id"),
                    TabId: null
                };
                attributes.push(obj);
            }
        });
        $("#CategoryAttributes .attribute").each(function () {
            if ($(this).is(":checked")) {
                if (!$(this).parent().parent().parent().parent().find(".Newlegend input[type='checkbox']").is(":checked")) {
                    var obj = {
                        AttributeId: $(this).attr("data-id"),
                        GroupId: $(this).parent().parent().parent().parent().attr("data-id"),
                        TabId: $(this).parent().parent().attr("tabid")
                    };
                    attributes.push(obj);
                }
            }
        });
        if (attributes.length > 0) {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/ProductAttributeGroups/AddAttribute/',
                data: JSON.stringify({ attributes: attributes, GroupId: $("#GroupId").val(), TabId: $("#TabId").val() }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {

                        $($this).parent().parent().append("<div class='catmsg'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:green!important;color:#fff!important;position:fixed;width:200px;z-index:2;top:20%;left:0'> " + response.Message + "</p></div>");
                        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); showAttributes($("#tree .current-cat")) });

                    }
                    else {
                        $($this).parent().parent().append("<div class='catmsg'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important;position:fixed;width:200px;z-index:2;top:20%;left:0'> " + response.Message + "</p></div>");
                        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });

                    }
                    $(".loading").hide();
                }
            });

        }
        else {
            $($this).parent().parent().append("<div class='catmsg'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important;position:fixed;width:200px;z-index:2;top:20%;left:0'>چیزی انتخاب نشده است!</p></div>");
            $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
        }
    });

    //Remove Attribute From Category
    $(".x_panel").on("click", "#CurrentCategoryAttributes .remove-att", function () {
        var $this = $(this);
        if (confirm("آیا مطمئن هستید؟")) {
            var attributes = [];
            attributes.push($this.parent().parent().attr("id"));
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/ProductAttributeGroups/RemoveAttribute/',
                data: JSON.stringify({ AttributeId: attributes, GroupId: $this.parent().parent().parent().parent().attr("data-id") }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {

                        $this.parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:green!important;color:#fff!important'> " + response.Message + "</p></div>");
                        $this.remove();
                        $(".catmsg").delay(1000).fadeOut(300, function () { showAttributes($("#tree .current-cat")); });
                    }
                    else {
                        $this.parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                        $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });

                    }
                    $(".loading").hide();
                }
            });
        }
    });

    $(".x_panel").on("click", "#CurrentCategoryAttributes .remove-Multi-att", function () {
        var $this = $(this);
        if (confirm("آیا مطمئن هستید؟")) {
            var attributes = [];
            $this.parent().parent().find(".attributeItem input[type='checkbox']:checked").each(function () {
                attributes.push($(this).attr("data-id"));
            });
            if (attributes.length > 0) {
                $(".loading").show();
                $.ajax({
                    type: "POST",
                    url: '/Admin/ProductAttributeGroups/RemoveAttribute/',
                    data: JSON.stringify({ AttributeId: attributes}),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.statusCode == 400) {

                            $this.parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:green!important;color:#fff!important'> " + response.Message + "</p></div>");
                            $(".catmsg").delay(1000).fadeOut(300, function () { showAttributes($("#tree .current-cat")); });
                        }
                        else {
                            $this.parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                            $(".catmsg").delay(1000).fadeOut(300, function () { });

                        }
                        $(".loading").hide();
                    }
                });
            }
            else {
                $($this).parent().parent().append("<div class='catmsg'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important;position:fixed;width:200px;z-index:2;top:20%;left:0'>چیزی انتخاب نشده است!</p></div>");
                $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
            }
        }
    });

    $(".x_panel").on("click", "#CurrentCategoryAttributes .remove-group", function () {
        var $this = $(this);
        if (confirm("آیا مطمئن هستید؟")) {
            var Groups = [];
            Groups.push($this.parent().parent().parent().attr("data-id"));
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/ProductAttributeGroups/RemoveGroupAttribute/',
                data: JSON.stringify({ GroupId: Groups }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {

                        $this.parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:green!important;color:#fff!important'> " + response.Message + "</p></div>");
                        $this.remove();
                        $(".catmsg").delay(1000).fadeOut(300, function () { showAttributes($("#tree .current-cat")); });
                    }
                    else {
                        $this.parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                        $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });

                    }
                    $(".loading").hide();
                }
            });
        }
    });

    $(".x_panel").on("click", "#CurrentCategoryAttributes .remove-group-multi", function () {
        var $this = $(this);
        if (confirm("آیا مطمئن هستید؟")) {
            var Groups = [];
            $("#CurrentCategoryAttributes .List > div").each(function () {
                if ($(this).find(".Newlegend input[type='checkbox']").is(":checked"))
                    Groups.push($(this).attr("id"));
            });
            if (Groups.length > 0) {

                $(".loading").show();
                $.ajax({
                    type: "POST",
                    url: '/Admin/ProductAttributeGroups/RemoveGroupAttribute/',
                    data: JSON.stringify({ GroupId: Groups }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.statusCode == 400) {

                            $this.parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:green!important;color:#fff!important'> " + response.Message + "</p></div>");
                            $this.remove();
                            $(".catmsg").delay(1000).fadeOut(300, function () { showAttributes($("#tree .current-cat")); });
                        }
                        else {
                            $this.parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                            $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });

                        }
                        $(".loading").hide();
                    }
                });
            }
            else {
                $($this).parent().parent().append("<div class='catmsg'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important;position:fixed;width:200px;z-index:2;top:20%;left:0'>چیزی انتخاب نشده است!</p></div>");
                $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
            }
        }
    });

    //Edit Group Of Category
    $(".x_panel").on("click", "#CurrentCategoryAttributes .Edit-Group", function () {
        var $this = $(this);

        $("#Edit-Dialog .ProductAttributeLink").attr("href", "/Admin/ProductAttributes/Edit/" + $this.parent().parent().attr("id"));

        //group
        $("#Edit-Dialog .Edit-Group-btn").attr("data-id", $this.parent().parent().attr("id"));
        if ($this.parent().parent().parent().parent().attr("data-id") == undefined) {
            $("#Edit-Dialog .Edit-Group-btn").attr("data-current-Groupid", null);
            $('select[name=GroupId2]').val(null);
            $('select[name=GroupId2]').selectpicker('refresh');
        }
        else {
            $("#Edit-Dialog .Edit-Group-btn").attr("data-current-Groupid", $this.parent().parent().parent().parent().attr("data-id"));
            $('select[name=GroupId2]').val($this.parent().parent().parent().parent().attr("data-id"));
            $('select[name=GroupId2]').selectpicker('refresh');
        }

        //tab
        $("#Edit-Dialog .Edit-Group-btn").attr("data-current-TabId", $this.parent().parent().attr("tabid"));
        $('select[name=TabId2]').val($this.parent().parent().attr("tabid"));
        $('select[name=TabId2]').selectpicker('refresh');

    });
    $(".x_panel").on("click", "#CurrentCategoryAttributes .Edit-Multi-Group", function () {
        var $this = $(this);

        if ($this.parent().parent().find(".attributeItem input[type='checkbox']:checked").length > 0) {
            var attid = "";
            $this.parent().parent().find(".attributeItem input[type='checkbox']:checked").each(function () {
                attid += $(this).attr("data-id") + ",";
            });

            //Group
            $("#Edit-Dialog .ProductAttributeLink").hide();
            $("#Edit-Dialog .Edit-Group-btn").attr("data-id", attid.substring(0, attid.length - 1));
            if ($this.parent().parent().parent().attr("data-id") == undefined) {
                $("#Edit-Dialog .Edit-Group-btn").attr("data-current-Groupid", null);
                $('select[name=GroupId2]').val(null);
                $('select[name=GroupId2]').selectpicker('refresh');
            }
            else {
                $("#Edit-Dialog .Edit-Group-btn").attr("data-current-Groupid", $this.parent().parent().parent().attr("data-id"));
                $('select[name=GroupId2]').val($this.parent().parent().parent().attr("data-id"));
                $('select[name=GroupId2]').selectpicker('refresh');
            }
            //Tab
            $("#Edit-Dialog .Edit-Group-btn").attr("data-current-TabId", $this.parent().next().attr("tabid"));
            $('select[name=TabId2]').val($this.parent().next().attr("tabid"));
            $('select[name=TabId2]').selectpicker('refresh');

            $('#Edit-Dialog').modal('show');

        }
        else {
            $($this).parent().parent().append("<div class='catmsg'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important;position:fixed;width:200px;z-index:2;top:20%;left:0'>چیزی انتخاب نشده است!</p></div>");
            $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
        }
    });
    $(".x_panel").on("click", "#CurrentCategoryAttributes .Edit-Group-btn", function () {
        var $this = $(this);
        {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/ProductAttributeGroups/EditAttributeGroup/',
                data: JSON.stringify({ AttributeId: $this.attr("data-id"),  CurrentGroupId: $this.attr("data-current-groupid"), GroupId: $("#GroupId2").val(), CurrentTabId: $this.attr("data-current-TabId"), TabId: $("#TabId2").val() }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {

                        $($this).parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:green!important;color:#fff!important'> " + response.Message + "</p></div>");
                        $(".catmsg").delay(3000).fadeOut(300, function () { showAttributes($("#tree .current-cat")); });
                    }
                    else {
                        $($this).parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                        $(".catmsg").delay(3000).fadeOut(300, function () { });

                    }
                    $(".loading").hide();
                }
            });
        }
    });

    //New--------------------------------------------
    //Dispaly Attributes
    $(".x_panel").on("click", ".NewFieldset .glyphicon-plus", function () {
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: '/Admin/ProductAttributeGroups/GetGroupAttribute/',
            data: JSON.stringify({  GroupId: ($this.parent().parent().parent().attr("data-id") == undefined ? $this.parent().parent().parent().attr("data-id") : $this.parent().parent().parent().attr("data-id")) }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $this.parent().parent().parent().find(".Attributes").fadeIn();
                    for (var i = 0; i < response.data.length; i++) {
                        $this.parent().parent().parent().find(".Attributes").append("<div tabid='" + response.data[i].TabId + "' id='" + response.data[i].AttributeId + "' class='attributeItem'><div class='col-lg-8 col-md-8 col-sm-8 col-xs-12'> <input class='attribute' type='checkbox' data-id='" + response.data[i].AttributeId + "' />  <label> " + response.data[i].Name + " </label></div><div class='col-lg-4 col-md-4 col-sm-4 col-xs-12 text-left'>" + ($this.parent().parent().parent().parent().parent().parent().attr("id") == "CurrentCategoryAttributes" ? "<span class='btn btn-default remove-att'  title='حذف'><span class='glyphicon glyphicon-trash'></span></span>&nbsp;&nbsp;<span class='btn btn-default  Edit-Group' data-toggle='modal' data-target='#Edit-Dialog'  title='تغییر دسته'><span class='glyphicon glyphicon-pencil'></span></span>" : "") + "</div><div class='clearfix'></div>  </div>");
                    }
                    $this.parent().parent().parent().find(".Attributes").append("<div class='clearfix'></div>");
                    $this.parent().append("<span class='glyphicon glyphicon-minus'></span>");
                    if ($this.parent().prev().find("input").is(":checked"))
                        $this.parent().parent().parent().find(".Attributes input[type='checkbox']").prop("checked", true);



                    //sort of Attribute in Group
                    var GroupId = $this.parent().parent().parent().attr("data-id");
                    $this.parent().parent().parent().parent().find(".NewFieldset").sortable({
                        items: ".attributeItem",
                        cursor: 'move',
                        opacity: 0.6,
                        placeholder: "ui-state-highlight",
                        update: function () {
                            $(".loading").show();
                            var order = $(this).sortable("toArray");
                            $.ajax({
                                crossDomain: true,
                                type: "POST",
                                url: "/Admin/ProductAttributeGroups/SortInGroup",
                                data: '{ids:"' + order + '",GroupId:"' + GroupId + '"}',
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (response) {
                                    if (response.statusCode == 400) {
                                        $("#CurrentCategoryAttributes .List .NewFieldset").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right;position:fixed;width:200px;z-index:2;top:20%;left:0' class='catmsg'> انجام شد . </p>").fadeIn(300);
                                        $(".catmsg").delay(1000).fadeOut(300, function () { });
                                    } else {
                                        $("#CurrentCategoryAttributes .List .NewFieldset").before("<p style='color:#fff;background-color:red;padding:5px;text-align:right;position:fixed;width:200px;z-index:2;top:20%;left:0' class='catmsg'> خطا </p>").fadeIn(300);;
                                        $(".catmsg").delay(1000).fadeOut(300, function () { });
                                    }

                                    $(".loading").hide();
                                }
                            });
                        }
                    });



                    $('html, body').animate({
                        scrollTop: $this.parent().parent().parent().offset().top
                    }, 700);

                    $this.remove();


                }
                else {
                    $($this).parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                    $(".catmsg").delay(1000).fadeOut(300, function () { });

                }

                $(".loading").hide();
            }
        });
    });
    $(".x_panel").on("click", ".NewFieldset .glyphicon-minus", function () {
        $(this).parent().parent().parent().find(".Attributes .attributeItem ").remove();
        $(this).parent().parent().parent().find(".Attributes").fadeOut();
        $(this).parent().append("<span class='glyphicon glyphicon-plus'></span>");
        $(this).remove();
    });
    $(".x_panel").on("change", ".List .Newlegend input[type='checkbox']", function () {
        if ($(this).is(":checked"))
            $(this).parent().parent().parent().parent().find(".Attributes input[type='checkbox']").prop("checked", true);
        else
            $(this).parent().parent().parent().parent().find(".Attributes input[type='checkbox']").prop("checked", false);
    });
    $(".x_panel").on("change", ".List .Attributes input[type='checkbox']", function () {
        var checkedCount = $(this).parent().parent().parent().find("input[type='checkbox']:checked").length;
        var allcount = $(this).parent().parent().parent().find("input[type='checkbox']").length;

        if (checkedCount == allcount)
            $(this).parent().parent().parent().parent().find(".Newlegend input[type='checkbox']").prop("checked", true);
        else
            $(this).parent().parent().parent().parent().find(".Newlegend input[type='checkbox']").prop("checked", false);
    });

    //Search Attribute
    $(".x_panel").on("click", ".SearchAttribute .glyphicon-chevron-down", function () {
        $(this).parent().next().fadeIn();
        $(this).parent().prepend("<span class='glyphicon glyphicon-chevron-up'></span>");
        $(this).remove();
    });
    $(".x_panel").on("click", ".SearchAttribute .glyphicon-chevron-up", function () {
        $(this).parent().next().fadeOut();
        $(this).parent().prepend("<span class='glyphicon glyphicon-chevron-down'></span>");
        $(this).remove();
    });
    $(".x_panel").on("click", "#SearchBoxCat .search", function () {
        SearchAttribute($(this));
    });
    $(".x_panel").on("keypress", "#SearchBoxCat input[type='text']", function (e) {
        if (e.which == 13) {
            SearchAttribute($(this));
        }
    });
    $(".x_panel").on("click", "#SearchBoxCat .showall", function () {
        showAttributes($("#tree .current-cat"));

    });
    $(".x_panel").on("click", "#SearchBoxSubCat .search ", function () {
        SearchAttributeSubCat($(this));
    });
    $(".x_panel").on("keypress", "#SearchBoxSubCat input[type='text']", function (e) {
        if (e.which == 13) {
            SearchAttributeSubCat($(this));
        }
    });
    $(".x_panel").on("click", "#SearchBoxSubCat .showall", function () {
        showAttributes($("#tree .current-cat"));

    });

    //Search Groupo Attribute
    $(".x_panel").on("click", ".SearchGroupAttribute .glyphicon-chevron-down", function () {
        $(this).parent().next().fadeIn();
        $(this).parent().prepend("<span class='glyphicon glyphicon-chevron-up'></span>");
        $(this).remove();
    });
    $(".x_panel").on("click", ".SearchGroupAttribute .glyphicon-chevron-up", function () {
        $(this).parent().next().fadeOut();
        $(this).parent().prepend("<span class='glyphicon glyphicon-chevron-down'></span>");
        $(this).remove();
    });
    $(".x_panel").on("click", "#SearchGroupBoxCat .search", function () {
        SearchGroupAttribute($(this));
    });
    $(".x_panel").on("keypress", "#SearchGroupBoxCat input[type='text']", function (e) {
        if (e.which == 13) {
            SearchGroupAttribute($(this));
        }
    });
    $(".x_panel").on("click", "#SearchGroupBoxCat .showall", function () {
        showAttributes($("#tree .current-cat"));

    });
    $(".x_panel").on("click", "#SearchGroupBoxSubCat .search ", function () {
        SearchAttributeGroupSubCat($(this));
    });
    $(".x_panel").on("keypress", "#SearchGroupBoxSubCat  input[type='text']", function (e) {
        if (e.which == 13) {
            SearchAttributeGroupSubCat($(this));
        }
    });
    $(".x_panel").on("click", "#SearchGroupBoxSubCat  .showall", function () {
        showAttributes($("#tree .current-cat"));

    });
});

function SearchAttribute($this) {

    if ($this.parent().parent().find("input[type='text']").val()) {
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: '/Admin/ProductAttributeGroups/SearchAttribute/',
            data: JSON.stringify({ AttributeName: $this.parent().parent().find("input[type='text']").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $("#CategoryAttributes .List .GroupAttribute").remove();
                    if (response.groups.length > 0) {
                        for (var i = 0; i < response.groups.length; i++) {
                            var html = "<div class='NewFieldset GroupAttribute'  data-id='" + response.groups[i].GroupId + "' data-cat-id='" + $this.parent().parent().parent().parent().attr("data-cat-id") + "'><div class='Newlegend' style='" + (response.groups[i].GroupId == null ? "background-color: brown;" : "") + "'><span>" + (response.groups[i].GroupId == null ? "بدون دسته" : response.groups[i].GroupName) + "</span></div><div style='display:block' class='Attributes'>";
                            for (var j = 0; j < response.Attributes.length; j++) {
                                if (response.Attributes[j].GroupId == response.groups[i].GroupId)
                                    html += "<div  tabid='" + response.Attributes[j].TabId + "' id='" + response.Attributes[j].AttributeId + "' class='attributeItem ui-sortable-handle'><div class='col-lg-8 col-md-8 col-sm-8 col-xs-12'> <input class='attribute' type='checkbox' data-id='" + response.Attributes[j].AttributeId + "'>  <label> " + response.Attributes[j].Name + " </label></div><div class='col-lg-4 col-md-4 col-sm-4 col-xs-12 text-left'></div><div class='clearfix'></div>  </div>";
                            }
                            html += "</div></div>";
                            $("#CategoryAttributes .List").append(html);

                        }
                    }
                    else
                        alert("چیزی پیدا نشد");


                }
                else {
                    $($this).parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                    $(".catmsg").delay(1000).fadeOut(300, function () { });

                }

                $(".loading").hide();
            }
        });
    }
}
function SearchAttributeSubCat($this) {

    if ($this.parent().parent().find("input[type='text']").val()) {
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: '/Admin/ProductAttributeGroups/SearchAttribute/',
            data: JSON.stringify({ AttributeName: $this.parent().parent().find("input[type='text']").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $("#CurrentCategoryAttributes .List .GroupAttribute").remove();
                    if (response.groups.length > 0) {
                        for (var i = 0; i < response.groups.length; i++) {
                            var html = "<div class='NewFieldset GroupAttribute'   data-id='" + response.groups[i].GroupId + "' data-cat-id='" + $this.parent().parent().parent().parent().attr("data-cat-id") + "'><div class='Newlegend' style='" + (response.groups[i].GroupId == null ? "background-color: brown;" : "") + "'> <div class='col-lg-9 col-md-9 col-sm-9 col-xs-12'><input type='checkbox'>&nbsp;<span>" + (response.groups[i].GroupId == null ? "بدون دسته" : response.groups[i].GroupName) + "</span></div><div class='col-lg-3 col-md-3 col-sm-3 col-xs-12 text-left'><span class='glyphicon glyphicon-trash remove-group' title='حذف این دسته'></span></div><div class='clearfix'></div><span></span></div><div style='display:block' class='Attributes'>";
                            for (var j = 0; j < response.Attributes.length; j++) {
                                if (response.Attributes[j].GroupId == response.groups[i].GroupId)
                                    html += "<div tabid='" + response.Attributes[j].TabId + "' id='" + response.Attributes[j].AttributeId + "' class='attributeItem ui-sortable-handle'><div class='col-lg-8 col-md-8 col-sm-8 col-xs-12'> <input class='attribute' type='checkbox' data-id='" + response.Attributes[j].AttributeId + "'>  <label> " + response.Attributes[j].Name + " </label></div><div class='col-lg-4 col-md-4 col-sm-4 col-xs-12 text-left'><span class='btn btn-default remove-att' title='حذف'><span class='glyphicon glyphicon-trash'></span></span>&nbsp;&nbsp;<span class='btn btn-default  Edit-Group' data-toggle='modal' data-target='#Edit-Dialog' title='تغییر دسته'><span class='glyphicon glyphicon-pencil'></span></span></div><div class='clearfix'></div>  </div>";
                            }
                            html += "</div></div>";
                            $("#CurrentCategoryAttributes .List").append(html);

                        }
                    }
                    else
                        alert("چیزی پیدا نشد");


                }
                else {
                    $($this).parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                    $(".catmsg").delay(1000).fadeOut(300, function () { });

                }

                $(".loading").hide();
            }
        });
    }
}

function SearchGroupAttribute($this) {

    if ($this.parent().parent().find("input[type='text']").val()) {
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: '/Admin/ProductAttributeGroups/SearchGroupAttribute/',
            data: JSON.stringify({ GroupName: $this.parent().parent().find("input[type='text']").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $("#CategoryAttributes .List .GroupAttribute").remove();
                    if (response.groups.length > 0) {
                        for (var i = 0; i < response.groups.length; i++) {
                            var html = "<div class='NewFieldset GroupAttribute'  data-id='" + response.groups[i].GroupId + "' data-cat-id='" + $this.parent().parent().parent().parent().attr("data-cat-id") + "'><div class='Newlegend' style='" + (response.groups[i].GroupId == null ? "background-color: brown;" : "") + "'><span>" + (response.groups[i].GroupId == null ? "بدون دسته" : response.groups[i].GroupName) + "</span></div><div style='display:block' class='Attributes'>";
                            for (var j = 0; j < response.Attributes.length; j++) {
                                if (response.Attributes[j].GroupId == response.groups[i].GroupId)
                                    html += "<div  tabid='" + response.Attributes[j].TabId + "' id='" + response.Attributes[j].AttributeId + "' class='attributeItem ui-sortable-handle'><div class='col-lg-8 col-md-8 col-sm-8 col-xs-12'> <input class='attribute' type='checkbox' data-id='" + response.Attributes[j].AttributeId + "'>  <label> " + response.Attributes[j].Name + " </label></div><div class='col-lg-4 col-md-4 col-sm-4 col-xs-12 text-left'></div><div class='clearfix'></div>  </div>";
                            }
                            html += "</div></div>";
                            $("#CategoryAttributes .List").append(html);

                        }
                    }
                    else
                        alert("چیزی پیدا نشد");


                }
                else {
                    $($this).parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                    $(".catmsg").delay(1000).fadeOut(300, function () { });

                }

                $(".loading").hide();
            }
        });
    }
}

function SearchAttributeGroupSubCat($this) {
    if ($this.parent().parent().find("input[type='text']").val()) {
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: '/Admin/ProductAttributeGroups/SearchGroupAttribute/',
            data: JSON.stringify({ GroupName: $this.parent().parent().find("input[type='text']").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $("#CurrentCategoryAttributes .List .GroupAttribute").remove();
                    if (response.groups.length > 0) {
                        for (var i = 0; i < response.groups.length; i++) {
                            var html = "<div class='NewFieldset GroupAttribute'   data-id='" + response.groups[i].GroupId + "' data-cat-id='" + $this.parent().parent().parent().parent().attr("data-cat-id") + "'><div class='Newlegend' style='" + (response.groups[i].GroupId == null ? "background-color: brown;" : "") + "'> <div class='col-lg-9 col-md-9 col-sm-9 col-xs-12'><input type='checkbox'>&nbsp;<span>" + (response.groups[i].GroupId == null ? "بدون دسته" : response.groups[i].GroupName) + "</span></div><div class='col-lg-3 col-md-3 col-sm-3 col-xs-12 text-left'><span class='glyphicon glyphicon-trash remove-group' title='حذف این دسته'></span></div><div class='clearfix'></div><span></span></div><div style='display:block' class='Attributes'>";
                            for (var j = 0; j < response.Attributes.length; j++) {
                                if (response.Attributes[j].GroupId == response.groups[i].GroupId)
                                    html += "<div tabid='" + response.Attributes[j].TabId + "' id='" + response.Attributes[j].AttributeId + "' class='attributeItem ui-sortable-handle'><div class='col-lg-8 col-md-8 col-sm-8 col-xs-12'> <input class='attribute' type='checkbox' data-id='" + response.Attributes[j].AttributeId + "'>  <label> " + response.Attributes[j].Name + " </label></div><div class='col-lg-4 col-md-4 col-sm-4 col-xs-12 text-left'><span class='btn btn-default remove-att' title='حذف'><span class='glyphicon glyphicon-trash'></span></span>&nbsp;&nbsp;<span class='btn btn-default  Edit-Group' data-toggle='modal' data-target='#Edit-Dialog' title='تغییر دسته'><span class='glyphicon glyphicon-pencil'></span></span></div><div class='clearfix'></div>  </div>";
                            }
                            html += "</div></div>";
                            $("#CurrentCategoryAttributes .List").append(html);

                        }
                    }
                    else
                        alert("چیزی پیدا نشد");


                }
                else {
                    $($this).parent().parent().append("<div class='catmsg' style='position:fixed;width:200px;z-index:2;top:20%;left:0'><br/><br/><br/><br/><p class='alert alert-info' style='background-color:red!important;color:#fff!important'> " + response.Message + "</p></div>");
                    $(".catmsg").delay(1000).fadeOut(300, function () { });

                }

                $(".loading").hide();
            }
        });
    }
}
function showAttributes(cat) {

    $(".loading").show(); NProgress.start();

    //Show Attribute
    $.ajax({
        type: "POST",
        url: "/Admin/ProductAttributeGroups/GetCatAttribute",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $("#CatAttributes").html(response.data);
            $(".loading").hide();
            $("#CurrentCategoryAttributes h4").text(" خصوصیات فعلی ");
            $("#CategoryAttributes h4").text(" خصوصیات قابل انتخاب  ");
            $('#GroupId,#GroupId2,#TabId2,#TabId').selectpicker('refresh');
            //Attribute Group
            $("#GroupId").change(function () {
                if ($(this).val() != "") {
                    //$("#ProductGroups").addClass("hide");
                    $("#GroupText").text("");
                }
                else {
                    //$("#ProductGroups").removeClass("hide");
                }
            });
            //sort of Attribute Group
            $("#CurrentCategoryAttributes .List").sortable({
                items: ".rowItem",
                cursor: 'move',
                opacity: 0.6,
                placeholder: "ui-state-highlight",
                update: function () {
                    $(".loading").show();
                    var order = $(this).sortable("toArray");
                    $.ajax({
                        crossDomain: true,
                        type: "POST",
                        url: "/ProductAttributeGroups/SortGroup",
                        data: '{ids:"' + order + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 400) {
                                $("#CurrentCategoryAttributes .List").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right;position:fixed;width:200px;z-index:2;top:20%;left:0' class='catmsg'> انجام شد . </p>").fadeIn(300);
                                $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                            } else {
                                $("#CurrentCategoryAttributes .List").before("<p style='color:#fff;background-color:red;padding:5px;text-align:right;position:fixed;width:200px;z-index:2;top:20%;left:0' class='catmsg'> خطا </p>").fadeIn(300);;
                                $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                            }
                            $(".loading").hide(); NProgress.done();
                        }
                    });
                }
            });
            //hide modal
            $('.modal-backdrop').remove(); $('body').removeClass('modal-open');
            $.ajax({
                ascync: false,
                type: "POST",
                url: "/ProductAttributeGroups/GetGroups",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $("#GroupId").prev().find(".dropdown-menu li").hide();
                    $("#GroupId").prev().find(".dropdown-menu li:first").show();
                    for (var i = 0; i < response.GroupIds.length; i++) {
                        var selectedIndex = $("#GroupId option[value=" + response.GroupIds[i] + "]").index();
                        $("#GroupId").prev().find(".dropdown-menu li:eq(" + selectedIndex + ")").show();

                    }


                    $(".loading").hide(); NProgress.done();

                }
            });
        }
    });



}