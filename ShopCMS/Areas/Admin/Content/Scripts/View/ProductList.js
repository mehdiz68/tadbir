$(document).ready(function () {

 $(".PageSizeSelector").change(function(){
     $("#PageSize").val($(this).val());
     $("#Search-Form").submit();
 });

});