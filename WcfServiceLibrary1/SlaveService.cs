﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CreatorLibrary;
using UserStorageServiceLibrary;

namespace WcfServiceLibrary1
{
  /// <summary>
  /// Slave service for deployng n master nodes of user storage service
  /// </summary>
  class SlaveService : IWcfService
  {
    /// <summary>
    /// Instance of master slave service deployer
    /// </summary>
    public static UserStorageAppDeployer msDeployer;

    static SlaveService()
    {
      Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var sections = cfg.Sections;
      var settings = (InitialSettingsConfigSection)sections["initialSettings"];
      var nodes = settings.ServiceNodesItems;
      int mCount = nodes[0].Count;
      int sCount = nodes[1].Count;


      msDeployer = new UserStorageAppDeployer(0, sCount, nodes[1].FirstPort);

      msDeployer.DeployUserStorageApp();
      ConnectToMaster(nodes[0].FirstPort);
    }

    /// <summary>
    /// Slave service cant do adding of user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public int Add(UserContext user)
    {
      throw new Exception("Operation not allowed");
    }

    /// <summary>
    /// Slave service cant do removing of user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool Remove(UserContext user)
    {
      throw new Exception("Operation not allowed");
    }
    /// <summary>
    /// Slave service cant do removing of user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool RemoveById(int userId)
    {
      throw new Exception("Operation not allowed");
    }

    /// <summary>
    /// Search user
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public IEnumerable<UserContext> Search(UserContext ctx)
    {
      IEnumerable<User> users = msDeployer.slaves[0].service.GetAll();
      if (ctx.FirstName != null)
      {
        users = users.Where(u => u.FirstName == ctx.FirstName);
      }
      if (ctx.LastName != null)
      {
        users = users.Where(u => u.LastName == ctx.LastName);
      }
      if (ctx.DateOfBirth != default(DateTime))
      {
        users = users.Where(u => u.BirthDate == ctx.DateOfBirth);
      }
      users = users.Where(u => u.Gender == ctx.Gender);

      return users.Select(u => ContextConverter.UserToContext(u));
    }


    /// <summary>
    /// Search user by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public UserContext SearchById(int id)
    {
      User user = msDeployer.slaves[0].service.FindById(id);
      return ContextConverter.UserToContext(user);
    }

    private static void ConnectToMaster(int firstMasterPort)
    {
      foreach (var slave in msDeployer.slaves)
      {
        slave.SetMasterPort(firstMasterPort);
      }
    }
  }
}
