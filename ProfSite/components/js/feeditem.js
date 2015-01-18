// Placeholder for facebook feed item javascript
console.log("feed item");
$('.btn-twitter-reply').on("click", function() {
	var post = $(this).closest('.social-post');
	post.find('.post-reply').removeClass('collapsed');
	post.find('.post-reply').find('textarea').focus();
});