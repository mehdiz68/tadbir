$(document).ready(function () {
    $("input[name='OnlineType']").next("div").toggleClass("hide", !$("input[name='OnlineType']").is(":checked"));
    $("input[name='InterfaceType']").next("div").toggleClass("hide", !$("input[name='InterfaceType']").is(":checked"));
    $("input[name='OnlineType']").change(function () {
        if($(this).is(":checked"))
            $(this).parent("div").find("> div").removeClass("hide");
        else
            $(this).parent("div").find("> div").addClass("hide");

    });
    $("input[name='InterfaceType']").change(function () {
        $(this).parent("div").find("> div").toggleClass("hide");
    });
    $("input[name='InterfaceType']").change(function () {
        if (!$(this).is("checked"))
            $("input[name='MerchantId']").val('');
    });
    $("#BankId,#NewBankId").change(function () {
        if($(this).val()==1)
        {
            $("input[name='OnlineType']").prop("disabled", false);
            if ($("input[name='OnlineType']").is(":checked"))
                $("input[name='OnlineType']").parent("div").find("> div").removeClass("hide");
        }
        else
        {
            //$("input[name='OnlineType']").prop("checked", false);
            //$("input[name='OnlineType']").prop("disabled", true);
            //$("input[name='OnlineType']").parent("div").find("> div").addClass("hide");
          
        }
    });
});