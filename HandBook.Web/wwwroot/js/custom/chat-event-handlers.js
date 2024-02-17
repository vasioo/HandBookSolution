$(document).on('input', '#messageText', function () {
    var $submitButton = $(document).find('#submitButton');

    if ($(this).val().trim() === '') {
        $submitButton.prop('disabled', true).addClass('disabled');
    } else {
        $submitButton.prop('disabled', false).removeClass('disabled');
    }
});
//$(document).ready(function () {
//    $(window).scroll(async function () {
//        if ($(window).scrollTop() <= 0) {
//            await loadOlderChatMessages();
//        }
//    });
//});

//async function loadOlderChatMessages() {
//    var currentPage = parseInt($("#currentPage").val());

//    try {
//        const response = await fetch(`/Mesages?handler=Chat&userName=${userName}&currentPage=${currentPage}`);
//        const partialView = await response.text();

//        $("#chat").prepend(partialView);

//        $("#currentPage").val(currentPage + 1);
//    } catch (error) {
//        console.error('Error loading older chat messages:', error);
//    }
//}