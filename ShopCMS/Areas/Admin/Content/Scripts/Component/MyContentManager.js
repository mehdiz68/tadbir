
; (function ($) {
    $.fn.extend({
        donetyping: function (callback, timeout) {
            timeout = timeout || 1e3; // 1 second default timeout
            var timeoutReference,
                doneTyping = function (el) {
                    if (!timeoutReference) return;
                    timeoutReference = null;
                    callback.call(el);
                };
            return this.each(function (i, el) {
                var $el = $(el);
                // Chrome Fix (Use keyup over keypress to detect backspace)
                // thank you @palerdot
                $el.is(':input') && $el.on('keyup keypress', function (e) {
                    // This catches the backspace button in chrome, but also prevents
                    // the event from triggering too premptively. Without this line,
                    // using tab/shift+tab will make the focused element fire the callback.
                    if (e.type == 'keyup' && e.keyCode != 8) return;

                    // Check if timeout has been set. If it has, "reset" the clock and
                    // start over again.
                    if (timeoutReference) clearTimeout(timeoutReference);
                    timeoutReference = setTimeout(function () {
                        // if we made it here, our timeout has elapsed. Fire the
                        // callback
                        doneTyping(el);
                    }, timeout);
                }).on('blur', function () {
                    // If we can, fire the event since we're leaving the field
                    doneTyping(el);
                });
            });
        }
    });
})(jQuery);

function CreateContents(container) {


    $(".ajax-form-manager").attr("data-ajax-success", "$('.loading').hide();CreateContents('" + container + "')");
    Init_Contents(container);
    
     
}
function Init_Contents(container) {

    $("#ContentManagerTree #ContentManagerTreeContainer li span").click(function () {
        var PostUrl = $(this).attr("data-url");
        $('.loading').show();
        $.ajax({
            type: 'GET',
            cache: false,
            async: true,
            url: PostUrl,
            UpdateTargetId: "ContentView",
            success: function (html) {
                 $('.loading').hide();
                 $('#ContentView').empty();
                $('#ContentView').html(html);
                Init_Contents(container);
            },
            error: function () { $('.loading').hide(); alert("error"); }
        });
    });
    $("#ContentManagerPagination .pagination li a").click(function (e) {
        e.preventDefault();
        var PostUrl = $(this).attr("href");
        $('.loading').show();
        $.ajax({
            type: 'GET',
            cache: false,
            async: true,
            url: PostUrl,
            UpdateTargetId: "ContentView",
            success: function (html) {
                $('.loading').hide();
                $('#ContentView').empty();
                $('#ContentView').html(html);
                Init_Contents(container);
            },
            error: function () { $('.loading').hide(); alert("error"); }
        });
    });
    $(".AddForLinkContent").click(function () {
        var $this = $(this);
        $("." + container).html("<p class='text text-success' data-id='" + $this.attr("data-Id") + "'> <span class='glyphicon glyphicon-remove' style='color:red;cursor:pointer'></span> " + $this.attr("data-Title") + "</p>");
        Cancel_btn_handler_ContentManager();
    });
    $(".LinkContent").on("click", ".glyphicon-remove", function () {
        $(this).parent().remove();
    });

    $("#ContentManager button[id=Cancel_btn_ContentManager]").click(function () {
        Cancel_btn_handler_ContentManager()
    });
   
    $("#SearchPageShow").click(function () {
        $("#SearchPage").slideToggle();
        $("#SearchPageShow").toggleClass("hilightSearch");
    });
    $.blockUI.defaults.overlayCSS = {
        backgroundColor: '#000',
        opacity: 0.6
    };
    $.blockUI.defaults.css = {
        padding: 0,
        margin: 5,
        width: '98%',
        top: '1%',
        left: '1%',
        color: '#000',
        border: '3px solid #aaa',
        backgroundColor: '#fff'
    };
    $.blockUI({ message: $('#ContentView') });

    $("#Cancel_btn_SearchContentManager").click(function () {
        $("#SearchPage").slideUp();
        $("#SearchPageShow").removeClass("hilightSearch");
    });

}

function Cancel_btn_handler_ContentManager() {
    $('#ContentView').empty();
    $.unblockUI();
}

$(document).on('keydown', function (e) {
    if (e.keyCode === 27) { // ESC
        Cancel_btn_handler_ContentManager();
    }
}); 


$(document).ready(function () {
    $(".timeSelector").click(function () {
        var input = $("#" + $(this).attr("data-id-selector"));
        var top=input.offset().top;
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
            if(input.val())
                input.val(input.val().substring(0, 10) + " " + hour + ":" + minute);
            else
                input.val("1396/01/01 " + hour + ":" + minute);

        });
        $(".MinuteSelector").change(function () {
            var minute= $(this).val();
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