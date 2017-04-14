#if !__MOBILE__
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using System.Data.Entity;

using Microsoft.Azure.Mobile.Server;

namespace NomadCode.Azure.Controllers
{
	public class AzureEntityController<TEntity, TContext> : TableController<TEntity>
		where TEntity : AzureEntity
		where TContext : DbContext, new()
	{
		protected override void Initialize (HttpControllerContext controllerContext)
		{
			base.Initialize (controllerContext);

			var context = new TContext ();

			DomainManager = new EntityDomainManager<TEntity> (context, Request);
		}

		// GET tables/{TEntity}
		public IQueryable<TEntity> GetAll () => Query ();


		// GET tables/{TEntity}/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public SingleResult<TEntity> Get (string id) => Lookup (id);


		// PATCH tables/{TEntity}/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task<TEntity> Patch (string id, Delta<TEntity> patch) => UpdateAsync (id, patch);


		// POST tables/{TEntity}
		public async Task<IHttpActionResult> Post (TEntity item)
		{
			TEntity current = await InsertAsync (item);
			return CreatedAtRoute ("Tables", new { id = current.Id }, current);
		}

		// DELETE tables/{TEntity}/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task Delete (string id) => DeleteAsync (id);
	}
}
#endif