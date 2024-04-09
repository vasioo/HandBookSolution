function likeButtonClick(Id) {
    $.ajax({
        type: "POST",
        url: "/Home/IncrementOrDecrementLikeCount",
        data: { itemId: Id },
        success: function (result) {
            var likeCountElement = $('.likeCount[data-item-id="' + Id + '"]');
            likeCountElement.html(result + ' <i class="fa-solid fa-heart liked fa-sm" style="color: #ff0000;"></i>');
            likeCountElement.toggle(result > 0);
        },
        error: function (result) {
            alert("Error: " + result.statusText);
        }
    });
}

function like(event, button, Id) {
    var card = button.closest(".card");
    var cardId = card.dataset.id;
    var count = parseInt(button.dataset.count) || 0;
    var likedCards = JSON.parse(sessionStorage.getItem("likedCards")) || {};

    var heartIcon = button.querySelector("i.fa-heart");


    if (heartIcon.classList.contains("liked")) {
        count--;
        button.dataset.count = count;
        button.innerHTML = ` <i class="fa-regular fa-heart fa-xl" style="color: #000;"></i>`;
        likeButtonClick(Id);
        delete likedCards[cardId];
    } else {
        count++;
        button.dataset.count = count;
        button.innerHTML = ` <i class="fa-solid fa-heart fa-xl liked" style="color: #ff0000;"></i>`;
        likeButtonClick(Id);
        likedCards[cardId] = true;
    }

    sessionStorage.setItem("likedCards", JSON.stringify(likedCards));
    event.stopPropagation();

}

var offsetPost = 0;
var loadingPost = false;

var offset = 0;

function loadMorePosts() {
    $.ajax({
        type: "POST",
        url: "/Home/LoadMorePosts",
        data: { offset: offsetPost },
        success: function (data) {
            var posts = JSON.parse(data);

            if (posts.length > 0) {
                offsetPost += posts.length;

                posts.forEach(function (post) {
                    var postString = '';
                    offsetPost += posts.length;

                    var link = "https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-" + post.creatorUserName + ".png";

                    var imgTag = '<img src="' + link + '" class="profile-image-class" alt="Image not found" style="border-radius:30px; width:60px;height:60px; margin-right:10px;" />';


                    var postHtml = '<div class="card col-7 mt-3" data-post-id="' + post.id + '">' +
                        '<div class="card-header border-bottom mb-2">' +
                        '<div class="row justify-content-between w-100">' +
                        '<div class="profile-column-post d-flex align-items-center ">' +
                        '<a class="" asp-controller="Home" asp-action="Account" asp-route-username="' + post.creatorUserName + '">' +
                        imgTag +
                        '</a>' +
                        '<div>' +
                        '<a class="card-title h5 cr-us-name" asp-controller="Home" asp-action="Account" asp-route-username="' + post.creatorUserName + '">' +
                        '&#64;' + post.creatorUserName +
                        '</a>' +
                        '<div class="custom-date">' + getTimeDisplay(post.time) + '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +

                        '<img src="data:image;base64,' + base64Image + '" style="border-radius:6px; height:500px" alt="Image" class="fit-image">' +
                        '<div class="card-body p-0 m-0">' +
                        '<div class="row justify-content-center p-0 m-0">' +
                        '<div class="col-10 pt-3">' +
                        '<div class="d-flex justify-content-between">' +
                        '<div>' +
                        '<span class="likeCount" data-item-id="' + post.id + '" style="display: ' + (post.amountOfLikes > 0 ? 'block' : 'none') + '">' +
                        post.amountOfLikes + ' <i class="fa-solid ' + likeButtonIcon + '" style="color: #ff0000;"></i>' +
                        '</span>' +
                        '</div>' +
                        '<div>' +
                        '<span class="commentsCount" data-item-id="' + post.id + '">' + commentsCount + ' ' + commentsText + '</span>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +

                        '<div class="d-flex justify-content-center">' +
                        '<hr class="col-11">' +
                        '</div>' +

                        '<div class="row">' +
                        '<div class="col-12 d-flex justify-content-around">' +
                        '<div class="col-1 pt-2 text-center pb-3">' +
                        '<button class="likeButton" data-count="' + post.amountOfLikes + '">' +
                        '<i class="fa-regular ' + likeButtonIcon + '" style="color: #000;"></i>' +
                        '</button>' +
                        '<br>' +
                        '</div>' +
                        '<div class="col-1 pt-2 text-center pb-3">' +
                        '<button class="commentButton">' +
                        '<i class="fa-regular fa-comment fa-xl"></i>' +
                        '</button>' +
                        '<br>' +
                        '</div>' +
                        '<div class="col-1 pt-2 text-center pb-3">' +
                        '<button class="shareButton" onclick="share(this,' + post.id + ')">' +
                        '<i class="fa-solid fa-share-from-square fa-xl"></i>' +
                        '</button>' +
                        '<br>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '<div class="row comment-section" style="display:none" data-post-id="' + post.id + '">' +
                        '<div class="col">' +
                        '<div class="col">' +
                        '<div class="input-group">' +
                        '<div class="border col-10 pl-1 pr-1 m-0">' +
                        '<textarea class="comments-text w-100" style="max-height:400px" placeholder="Write a comment..."></textarea>' +
                        '</div>' +
                        '<div class="input-group-append align-self-end col-2">' +
                        '<button class="btn btn-primary submit-btn" type="button"> <i class="fas fa-right-long"></i></button>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '<div class="comment-section-regulation-div" id="comment-container-"' + post.id + '">' +
                        '</div>' +
                        '<div class="">' +
                        '<a class="load-more-comments">Load More Comments</a>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>';


                    $(".post-container").append(postString);
                });
            }

            loadingPost = false;
        },
        error: function (error) {
            console.log("Error loading more posts: " + error.responseText);
            loadingPost = false;
        }
    });
}
