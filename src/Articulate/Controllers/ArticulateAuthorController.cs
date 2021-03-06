using System;
using System.Linq;
using System.Web.Mvc;
using Articulate.Models;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Articulate.Controllers
{
    public class ArticulateAuthorController : ListControllerBase
    {
        /// <summary>
        /// Override and declare a NonAction so that we get routed to the Index action with the optional page route
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [NonAction]
        public override ActionResult Index(RenderModel model)
        {
            return Index(model, 0);
        }

        public ActionResult Index(RenderModel model, int? p)
        {
            //create a master model
            var masterModel = new MasterModel(model.Content);

            var listNodes = masterModel.RootBlogNode.Children("ArticulateArchive").ToArray();
            if (listNodes.Length == 0)
            {
                throw new InvalidOperationException("An ArticulateArchive document must exist under the root Articulate document");
            }

            var totalPosts = Umbraco.GetPostCount(model.Content.Name, listNodes.Select(x => x.Id).ToArray());

            PagerModel pager;
            if (!GetPagerModel(masterModel, totalPosts, p, out pager))
            {
                return new RedirectToUmbracoPageResult(model.Content.Parent, UmbracoContext);
            }

            var authorPosts = Umbraco.GetContentByAuthor(listNodes, model.Content.Name, pager);
            var author = new AuthorModel(model.Content, authorPosts, pager, totalPosts);
            
            return View(PathHelper.GetThemeViewPath(author, "Author"), author);
        }
    }
}