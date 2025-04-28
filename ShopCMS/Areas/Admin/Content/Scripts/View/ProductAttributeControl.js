$(document).ready(function () {

    //Remove selected File Control
    $("body").on("click", ".removePic", function () {
        $(this).next().find("img").removeAttr("src");
        $(this).next().find("input").val("");
        $(this).next().find("input").removeAttr("title");
    });

    //Prepare Product Multiple Value
    $("body").on("click", ".MultipleValuesContainer .btn", function () {
        $("#ErrorSummary").html("");
        var $this = $(this);
        var ProductAttribute = $(this).parent().parent().parent().find(".ProductAttribute");
        var DataType = parseInt(ProductAttribute.attr("data-type-id"));
        var value = GetAttributeValue(DataType, ProductAttribute);
        if (value != "")
            $this.parent().find(".MultipleValues").append(value);
    });
    $("body").on("click", ".MultipleValues .glyphicon-remove", function () {
        $(this).parent().remove();
    });
    $("body").on("click", ".glyphicon-remove", function () {
        $(this).parent().remove();
    });

    //date time
    $(document).ready(function () {
        $(".timeSelector").click(function () {
            var input = $("#" + $(this).attr("data-id-selector"));
            var top = input.offset().top;
            var left = input.offset().left;
            var heightElement = input.outerHeight(true); 0
            $(".timeSelectorPlugin").remove();
            $("body").prepend("<div class='timeSelectorPlugin' style='left:" + left + "px;top:" + (parseInt(top) + parseInt(heightElement)).toString() + "px'><lable>دقیقه</lable><select class='MinuteSelector'><option value='00'>00</option><option value='01'>01</option><option value='02'>02</option><option value='03'>03</option><option value='04'>04</option><option value='05'>05</option><option value='06'>06</option><option value='07'>07</option><option value='08'>08</option><option value='09'>09</option><option value='10'>10</option><option value='11'>11</option><option value='12'>12</option><option value='13'>13</option><option value='14'>14</option><option value='15'>15</option><option value='16'>16</option><option value='17'>17</option><option value='18'>18</option><option value='19'>19</option><option value='20'>20</option><option value='21'>21</option><option value='22'>22</option><option value='23'>23</option><option value='24'>24</option><option value='25'>25</option><option value='26'>26</option><option value='27'>27</option><option value='28'>28</option><option value='29'>29</option><option value='30'>30</option><option value='31'>31</option><option value='32'>32</option><option value='33'>33</option><option value='34'>34</option><option value='35'>35</option><option value='36'>36</option><option value='37'>37</option><option value='38'>38</option><option value='39'>39</option><option value='40'>40</option><option value='41'>41</option><option value='42'>42</option><option value='43'>43</option><option value='44'>44</option><option value='45'>45</option><option value='46'>46</option><option value='47'>47</option><option value='48'>48</option><option value='49'>49</option><option value='50'>50</option><option value='51'>51</option><option value='52'>52</option><option value='53'>53</option><option value='54'>54</option><option value='55'>55</option><option value='56'>56</option><option value='57'>57</option><option value='58'>58</option><option value='59'>59</option></select><span> : </span><select class='HourSelector'><option value='01'>01</option><option value='02'>02</option><option value='03'>03</option><option value='04'>04</option><option value='05'>05</option><option value='06'>06</option><option value='07'>07</option><option value='08'>08</option><option value='09'>09</option><option value='10'>10</option><option value='11'>11</option><option selected value='12'>12</option><option value='13'>13</option><option value='14'>14</option><option value='15'>15</option><option value='16'>16</option><option value='</'>17</option><option value='18'>18</option><option value='19'>19</option><option value='20'>20</option><option value='21'>21</option><option value='22'>22</option><option value='23'>23</option></select><p>بستن</p></div> ");
            if (input.val())
                input.val(input.val().substring(0, 10) + " 12:00");
            else
                input.val("1396/01/01 12:00");

            $(".HourSelector").change(function () {
                var hour = $(this).val();
                var minute = $(".MinuteSelector").val();
                if (input.val())
                    input.val(input.val().substring(0, 10) + " " + hour + ":" + minute);
                else
                    input.val("1396/01/01 " + hour + ":" + minute);

            });
            $(".MinuteSelector").change(function () {
                var minute = $(this).val();
                var hour = $(".HourSelector").val();
                if (input.val())
                    input.val(input.val().substring(0, 10) + " " + hour + ":" + minute);
                else
                    input.val("1396/01/01 " + hour + ":" + minute);

            });

            $(".timeSelectorPlugin p").click(function () {
                $(this).parent().remove();
                $("#datepicker").remove();
            });
        });
        $('.ProductAttribute[data-type-id=39] input').focusin(function () {
            if ($(this).val().indexOf(":") < 0 && $(this).val()) {
                $(this).val($(this).val() + " 12:00");
            }
        });
        $('.ProductAttribute[data-type-id=40] input').focusin(function () {
            if ($(this).val().indexOf(":") < 0 && $(this).val()) {
                $(this).val($(this).val() + " 12:00");
            }
        });
    });

});


function GetAttributeValue(DataType, selector) {
    var result = "";
    var CountValue = 0;
        switch (DataType) {
            case 1:
            case 2:
            case 3:
            case 13:
            case 14:
            case 16:
            case 17:
            case 18:
                if (selector.find("input").val()) {
                    result = "<span data-type='" + DataType + "' class='selectedValue'><span class='glyphicon glyphicon-remove' style='cursor:pointer'></span>" + selector.find("input").val() + "</span>";
                    selector.find("input").val("");
                    return result;
                }
                else {
                    $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                    $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                    $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.parent().find(".AttributeName").text() + " ، باید وارد شود </p>");
                    return "";
                }

            case 4:
                if (selector.find("textarea").val()) {
                    result = "<span data-type='" + DataType + "' class='selectedValue'><span class='glyphicon glyphicon-remove' style='cursor:pointer'></span>" + selector.find("textarea").val() + "</span>";
                    selector.find("textarea").val("");
                    return result;
                }
                else {
                    $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                    $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                    $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.parent().find(".AttributeName").text() + " ، باید وارد شود </p>");
                    return "";
                }
            case 7:
                if (selector.find("input").attr("title") != undefined) {
                    result = "<span data-type='" + DataType + "' data-value='" + selector.find("input").val() + "' class='selectedValue'><span class='glyphicon glyphicon-remove' style='cursor:pointer'></span><img src='" + selector.find("img").attr("src") + "' width='50' height='50' />" + selector.find("input").attr("title") + "</span>";
                    return result;
                }
                else {
                    $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                    $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                    $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.parent().find(".AttributeName").text() + " ، باید وارد شود </p>");
                    return "";
                }
            case 8:
            case 15:
            case 11:
            case 12:

                if (selector.find("select option:selected").attr("data-id") != "0") {
                    result = "<span data-type='" + DataType + "' data-value='" + selector.find("select option:selected").attr("data-id") + "' class='selectedValue'><span class='glyphicon glyphicon-remove' style='cursor:pointer'></span>" + selector.find("select option:selected").text() + "</span>";
                    return result;
                }
                else {
                    $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                    $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                    $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.parent().find(".AttributeName").text() + " ، باید وارد شود </p>");
                    return "";
                }

            case 9:
                if (selector.find(".LinkContent p").html() != undefined) {
                    result = "<span data-type='" + DataType + "' data-value='" + selector.find(".LinkContent p").attr("data-id") + "'  class='selectedValue'><span class='glyphicon glyphicon-remove' style='cursor:pointer'></span>" + selector.find(".LinkContent p").text() + "</span>";
                    return result;
                }
                else {
                    $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                    $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                    $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.parent().find(".AttributeName").text() + " ، باید وارد شود </p>");
                    return "";
                }
            case 10:
                if (selector.find(".LinkProduct p").html() != undefined) {
                    result = "<span data-type='" + DataType + "' data-value='" + selector.find(".LinkProduct p").attr("data-id") + "' class='selectedValue'><span class='glyphicon glyphicon-remove' style='cursor:pointer'></span>" + selector.find(".LinkProduct p").text() + "</span>";
                    return result;
                }
                else {
                    $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                    $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                    $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.parent().find(".AttributeName").text() + " ، باید وارد شود </p>");
                    return "";
                }
            default:
                if (selector.find("input").val()) {
                    result = "<span data-type='" + DataType + "' class='selectedValue'><span class='glyphicon glyphicon-remove' style='cursor:pointer'></span>" + selector.find("input").val() + "</span>";
                    selector.find("input").val("");
                    return result;
                }
                else {
                    $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                    $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                    $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.parent().find(".AttributeName").text() + " ، باید وارد شود </p>");
                    return "";
                }
        }
   
   
}


function GetValue(AttributeId, DataType, selector, isRelational) {
    var result = "";
        switch (DataType) {
            case 1:
            case 2:
            case 3:
            case 13:
            case 14:
            case 16:
            case 17:
            case 18:
                if (selector.find("input").val()) {
                    result = selector.find("input").val();
                    return result;
                }
                else {
                    if (selector.attr("data-isrequired").toLowerCase() == "true") {
                        $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);
                        $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                        $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.find(".AttributeName").text() + " ، باید وارد شود </p>");
                        return "nok";
                    }
                    return "";
                }

            case 4:
                if (selector.find("textarea").val()) {
                    result = selector.find("textarea").val();
                    return result;
                }
                else {
                    if (selector.attr("data-isrequired").toLowerCase() == "true") {
                        $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                        $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                        $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.find(".AttributeName").text() + " ، باید وارد شود </p>");
                        return "nok";
                    }
                    return "";
                }

            case 5:
                if (!selector.find("input").prop('disabled'))
                return (selector.find("input").is(":checked") ? "1" : "0");
                else
                return "";
            case 7:
                if (selector.find("input").attr("title") != undefined) {
                    return selector.find("input").val();
                }
                else {
                    if (selector.attr("data-isrequired").toLowerCase() == "true") {
                        $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                        $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                        $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.find(".AttributeName").text() + " ، باید وارد شود </p>");
                        return "nok";
                    }
                    return "";
                }
            case 8:
            case 15:
            case 11:
            case 12:
                if (selector.find("select option:selected").attr("data-id") != "0") {
                    return selector.find("select option:selected").attr("data-id");
                }
                else {
                    if (selector.attr("data-isrequired").toLowerCase() == "true") {
                        $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                        $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                        $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.find(".AttributeName").text() + " ، باید وارد شود </p>");
                        return "nok";
                    }
                    return "";
                }
            case 9:
                if (selector.find(".LinkContent p").html() != undefined) {
                    return selector.find(".LinkContent p").attr("data-Id");
                }
                else {
                    if (selector.attr("data-isrequired").toLowerCase() == "true") {
                        $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                        $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                        $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.find(".AttributeName").text() + " ، باید وارد شود </p>");
                        return "nok";
                    }
                    return "";
                }
            case 10:
                if (selector.find(".LinkProduct p").html() != undefined) {
                    return selector.find(".LinkProduct p").attr("data-Id");
                }
                else {
                    if (selector.attr("data-isrequired").toLowerCase() == "true") {
                        $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);;
                        $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                        $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.find(".AttributeName").text() + " ، باید وارد شود </p>");
                        return "nok";
                    }
                    return "";
                }
            default:
                if (selector.find("input").val()) {
                    result = selector.find("input").val();
                    return result;
                }
                else {
                    if (selector.attr("data-isrequired").toLowerCase() == "true") {
                        $(selector).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);
                        $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                        $("#ErrorSummary").prepend("<p class='alert alert-error'> " + selector.find(".AttributeName").text() + " ، باید وارد شود </p>");
                        return "nok";
                    }
                    return "";
                }
        }
    
}


function GetRawAttributeValue(DataType, selector) {
    switch (DataType) {
            case 1:
            case 2:
            case 3:
            case 13:
            case 14:
            case 16:
            case 17:
            case 18:
            return selector.text();
        case 4:
            return selector.text();
        case 7:
            return selector.attr("data-value");

            case 8:
            case 15:
            case 11:
            case 12:
            return selector.attr("data-value");

        case 9:
            return selector.attr("data-value");
        case 10:
            return selector.attr("data-value");

        case 5:
            return selector.text();
        default:
            return selector.text();
    }

}