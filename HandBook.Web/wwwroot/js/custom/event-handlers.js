$(document).on('click', '.load-more-comments', function (event) {

    const card = $(this).closest('.card');
    var postId = $(card).data('post-id');

    loadMoreComments(0, postId, event);
});

$(document).on('click', '.load-more-replies', function (event) {
    var commentId = $(this).closest('.comment-container').data('comment-id');

    const card = $(this).closest('.card');
    var postId = $(card).data('post-id');

    loadMoreComments(commentId, postId, event);
});

$(document).on('click', '.comment-like-button', function () {
    const comment = $(this).closest('.comment-container').find('.commentlikeCount');
    var postId = comment.data('item-id');

    likecom(this, postId);
});

$(document).on('click', '.append-reply-textbox', function (event) {
    var $nextSibling = $(this).closest('.comment-actions').next();

    var $replyButton = $(this);
    var $inputBoxContainer = $replyButton.closest('.comment-actions').next('.reply-input-box');

    if (!$nextSibling.hasClass('ibht')) {
        var neededId = $(this).attr('data-comment-id');
        var derivingFromUsername = $('.comment[data-comment-id="' + neededId + '"]').data('comment-username')
        var inputBoxHtml = `
            <div class="row ibht py-3" style="padding-left: 1.2em;">
                <div class="col-11" style="background:lightgray;border-radius: 15px;">
                    <div class="col reply-input-box">
                        <div class="replies-group d-flex justify-content-between">
                            <div class="col-11 pl-1 pr-1 m-0">
                                <textarea class="reply-text w-100" placeholder="Reply to ${derivingFromUsername}"></textarea>
                            </div>
                            <div class="input-group-append align-self-end">
                                <button class="btn submit-reply-btn" type="button" disabled>
                                    <i class="fa-solid fa-paper-plane" style="rotate:30deg"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
           </div>`;
        $replyButton.closest('.comment-actions').after(inputBoxHtml);
    } else {
        $nextSibling.remove();
    }
    $inputBoxContainer.toggle();

});

$(document).on('click', '.commentButton', function (event) {
    var itemId = this.id;
    toggleComments(this, itemId, event);
});

$(document).on('click', '.likeButton', function (event) {
    var itemId = this.id;
    like(event,this, itemId);
});

$(document).on('click', '.toggle-replies-btn', function () {
    var highestElement = $(this).parents('.comment-container').last();
    var commentId = highestElement.data('comment-id');

    var closestElement = $(this).closest('.comment-container');
    var commentReplyId = closestElement.data('comment-id');

    const card = this.closest('.card');
    var postId = $(card).data('post-id');

    var repliesContainer = this.nextElementSibling;

    var subContainerId = '.commentlikeCount[data-item-id="' + commentReplyId + '"]';
    var subRepliesContainer = $(`${subContainerId}`);

    if (subRepliesContainer.length > 0 && !subRepliesContainer.hasClass('comments-loaded')) {
        loadMoreComments(commentReplyId, postId, $(this));
        subRepliesContainer.addClass('comments-loaded');
    }

    if (repliesContainer != null && repliesContainer != undefined) {
        $(repliesContainer).toggle();
        if ($(repliesContainer).is(':visible')) {
            $(this).text('Hide Replies');
        } else {
            $(this).text('View Replies');
        }
    }

    if ($(this).hasClass('no-src')) {
        $('.comment-container[data-deriving-from="' + commentReplyId + '"]').toggle();
        var commentContainers = $('.comment-container[data-deriving-from="' + commentReplyId + '"]');

        if (commentContainers.is(':visible')) {
            $(this).text('Hide Replies');
        } else {
            $(this).text('View Replies');
        }
        if (!commentContainers || commentContainers.length === 0) {
            $(this).text('Hide Replies');
            return;
        }

    }
});

$(document).on('click', '.submit-btn', function () {
    var $commentContainer = $(this).closest('.comment-section');
    var $commentTextarea = $commentContainer.find('.comment-text');

    var commentContent = $commentTextarea.val();
    var postId = $commentContainer.data('post-id');

    var comment = {
        CommentContent: commentContent,
        DateOfCreation: new Date().toString(),
        PostId: postId
    };

    $.ajax({
        url: "/Home/AddOrRemoveAComment",
        method: 'POST',
        data: { commentsDTO: comment },
        dataType: 'json',
        success: function (response) {
            var commentDto = JSON.parse(response);
            var timestamp = new Date().getTime();

            var usernameLink = `https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-${commentDto.UserUsername}?timestamp=${timestamp}`;
            var alternativeLink = "/handbook/images/anonymousUser.png";
            var imgTag = '<img src="' + usernameLink + '" class="profile-image-class" alt="Image not found" onerror="this.onerror=null;this.src=\'' + alternativeLink + '\';" style="border-radius:30px; width:50px;height:50px; margin-right:10px;" />';

            var commentTextWrapper = `
                <div class="d-flex pb-3 pt-2"> ${commentDto.CommentContent}</div>
                <div class="row">
                    <span class="commentlikeCount" data-item-id="${commentDto.Id}">
                    </span>
                </div>
            `;

            var commentHtml = `
            <div class="comment-container row pt-2" data-comment-id="${commentDto.Id}" data-deriving-from="0">
                <div class="profile-column col-1 mx-2 p-0">
                   `+ imgTag + `
                </div>
                <div class="comment-column col-10 mx-2 p-0">
                    <div class="comment" data-comment-id="${commentDto.Id}" data-comment-username="${commentDto.UserUsername}">
                        <div class="comment-header rounded-top">
                            <div class="comment-username font-weight-bold">${commentDto.UserUsername}</div>
                            <div class="comment-time text-muted">Now</div>
                        </div>
                        <div class="comment-content rounded-bottom">
                           ${commentTextWrapper}
                        </div>
                        <div class="comment-actions mt-2">
                            <a class="comment-like-button btn text-decoration-none text-primary" data-count="1">
                                <i class="fa-regular fa-heart fa-xl com-btn" style="color: #000;" aria-hidden="true"></i>
                            </a>
                            <button class="append-reply-textbox btn" data-comment-id="${commentDto.Id}">
                                <i class="fa-solid fa-reply fa-xl" aria-hidden="true"></i>
                            </button>
                        </div>
                    </div>
                    <div class="sub-replies-container" id="sub-replies-container-${commentDto.Id}" style="display: none;">
                    </div>
                </div>
            </div>
        `;

            var neededId = `#comment-container-${commentDto.PostId}`;
            $(neededId).prepend(commentHtml);


        },
        error: function (error) {
            console.error('Error submitting comment:', error);
        }
    });
});

$(document).on('click', '.submit-reply-btn', function () {
    var $submitButton = $(this); 
    var $commentContainer = $submitButton.closest('.replies-group');
    var $repliesContainer = $submitButton.closest('.comment-container').find('.commentlikeCount');
    var commentDeriveFromId = $repliesContainer.data('item-id');
    var $commentTextarea = $commentContainer.find('.reply-text');
    var commentContent = $commentTextarea.val();
    var postId = $submitButton.closest('.comment-section').data('post-id');

    var comment = {
        CommentContent: commentContent,
        DateOfCreation: new Date().toString(),
        PostId: postId,
        CommentDeriveFromId: commentDeriveFromId
    };

    $.ajax({
        url: "/Home/AddOrRemoveAComment",
        method: 'POST',
        data: { commentsDTO: comment },
        dataType: 'json',
        success: function (response) {
            var commentDto = JSON.parse(response);

            var usernameLink = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-' + commentDto.UserUsername + '.png';
            var alternativeLink = "/handbook/images/anonymousUser.png";
            var imgTag = '<img src="' + usernameLink + '" alt="Image not found" onerror="this.onerror=null;this.src=\'' + alternativeLink + '\';" style="border-radius:30px; width:60px;height:60px; margin-right:10px;" />';
            var commentTextWrapper = `
                <div class="d-flex pb-3 pt-2">
                    <a class="custom-username-link" href="Home/Account?username=${commentDto.UserUsername}">
                        @${commentDto.UserUsername} &#160
                    </a>
                    ${commentDto.CommentContent}
                </div>
                <div class="row">
                    <span class="commentlikeCount" data-item-id="${commentDto.Id}">
                    </span>
                </div>
            `;
            var commentHtml = `
                <div class="comment-container row pt-2" data-comment-id="${commentDto.Id}" data-deriving-from="0">
                    <div class="profile-column col-1 mx-2 p-0">
                    `+ imgTag + `
                    </div>
                    <div class="comment-column col-10 mx-2 p-0">
                       <div class="comment" data-comment-id="${commentDto.Id}" data-comment-username="${commentDto.UserUsername}">
                           <div class="comment-header rounded-top">
                               <div class="comment-username font-weight-bold">${commentDto.UserUsername}</div>
                               <div class="comment-time text-muted">Now</div>
                           </div>
                           <div class="comment-content rounded-bottom">
                              ${commentTextWrapper}
                           </div>
                           <div class="comment-actions mt-2">
                               <a class="comment-like-button btn text-decoration-none text-primary" data-count="1">
                                   <i class="fa-regular fa-heart fa-xl com-btn" style="color: #000;" aria-hidden="true"></i>
                               </a>
                               <button class="append-reply-textbox btn" data-comment-id="${commentDto.Id}">
                                   <i class="fa-solid fa-reply fa-xl" aria-hidden="true"></i>
                               </button>
                           </div>
                       </div>
                    </div>
                    `;

            var predictedId = `#sub-replies-container-${commentDto.CommentDeriveFromId}`;

            if ($(predictedId).length) {
                $(predictedId).append(commentHtml);
                $(predictedId).css('display', 'block');
            } else {
                var parentContainer = $submitButton.closest('.sub-replies-container'); 
                parentContainer.prepend(commentHtml);
                $('html, body').animate({
                    scrollTop: parentContainer.offset().top
                }, 'fast');
            }

        },
        error: function (error) {
            console.error('Error submitting comment:', error);
        }
    });
});

$(document).on('input', '.comment-text', function () {
    var $submitButton = $(this).parent().next().find('.submit-btn');
    if ($(this).val().trim() === '') {
        $submitButton.prop('disabled', true);
    } else {
        $submitButton.prop('disabled', false);
    }
});

$(document).on('input', '.reply-text', function () {
    var $submitButton = $(this).parent().next().find('.submit-reply-btn');

    if ($(this).val().trim() === '') {
        $submitButton.prop('disabled', true);
    } else {
        $submitButton.prop('disabled', false);
    }
});

$(document).on('error', '.profile-image-class', function () {
    $(this).off('error').attr('src', '/handbook/images/anonymousUser.png');
}).on('load', '.profile-image-class', function () { });

$(document).ready(function () {
    $(document).find('.profile-image-class').each(function () {
        if (!this.complete || (typeof this.naturalWidth !== 'undefined' && this.naturalWidth === 0)) {
            $(this).trigger('error');
        }
    });
});

$(document).on
    ('textarea', 'input', function () {
    this.style.height = 'auto';

    this.style.height =
        (this.scrollHeight) + 'px';
});

$(window).scroll(function () {
    if ($(window).scrollTop() + $(window).height() >= $(".postContainer").height() - 200 && !loading) {
        loading = true;
        loadMorePosts();
    }
});