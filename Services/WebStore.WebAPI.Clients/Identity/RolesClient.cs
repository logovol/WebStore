﻿using WebStore.Interfaces;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Identity;

public class RolesClient : BaseClient
{
	public RolesClient(HttpClient Client) : base(Client, WebApiAddresses.V1.Identity.Roles)
	{

	}
}
