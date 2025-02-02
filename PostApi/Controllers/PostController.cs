using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PostApi.Dtos;
using PostApi.Models;
using PostApi.Repositories;


namespace PostApi.Controllers
{
    [Route("api/posts")]
    [ApiController]
    [Authorize] 
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public PostsController(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _postRepository.GetAllPosts();
            
            var postsDto = _mapper.Map<List<PostDto>>(posts);

            if (postsDto == null)
            {
                return NotFound("No posts found for the given user.");
            }
            
            return Ok(postsDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateDto postDto)
        {
            
            var userId = int.Parse(User.FindFirst("userId")?.Value);

            var post = _mapper.Map<Post>(postDto);
            post.UserId = userId;
            post.CreatedAt = DateTime.UtcNow;
            var createdPost = await _postRepository.CreatePostAsync(post);
            return CreatedAtAction(nameof(GetPostById), new { id = createdPost.Id }, createdPost);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpGet("my-posts")]
        public async Task<IActionResult> GetMyPosts()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);

            var posts = await _postRepository.GetMyPostsAsync(userId);

            if (posts == null || !posts.Any())
            {
                return NotFound("No posts found for the given user.");
            }

            var postsDto = _mapper.Map<List<PostDto>>(posts);
            return Ok(postsDto);
        }

        [HttpGet("friends-posts")]
        public async Task<IActionResult> GetFriendsPosts()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);

            var posts = await _postRepository.GetFriendsPostsAsync(userId);

            if (posts == null || !posts.Any())
            {
                return NotFound("No posts found for the given user's friends.");
            }

            var postsDto = _mapper.Map<List<PostDto>>(posts);
            return Ok(postsDto);
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);
            var post = await _postRepository.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            if (post.UserId != userId)
            {
                return Unauthorized("You can only delete your own posts");
            }
            await _postRepository.DeletePostAsync(post);

            return NoContent(); 
        }
    }
}