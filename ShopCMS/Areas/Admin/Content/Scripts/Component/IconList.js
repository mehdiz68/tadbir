 $(document).ready(function () {

        $(".fontawesome-icon-list a").click(function (e) {
            e.preventDefault();
            $("#Icon").val($(this).find("i").attr("class"));
            $("#Icon").val($("#Icon").val().replace("active", ""));
            $('#IconsModalLong').modal('toggle');
        });
        $("#Search-icon-list").keyup(function () {
            $(".fontawesome-icon-list i").removeClass("active");
            var $found = $(".fontawesome-icon-list i[class*='" + $(this).val() + "']");
            $found.addClass("active");
            if ($found.length>0)
                $("#IconsModalLong").stop().animate({ scrollTop:  $found[0].offsetParent.offsetTop }, 500, 'swing', function () { });
        });
    });