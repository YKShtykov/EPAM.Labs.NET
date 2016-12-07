using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CreatorLibrary;
using UserStorageServiceLibrary;

namespace WcfServiceLibrary1
{

  /// <summary>
  /// Master service for deployng n master nodes of user storage service
  /// </summary>
  public class MasterService : IWcfService
  {
    /// <summary>
    /// Instance of master slave service deployer
    /// </summary>
    public static UserStorageAppDeployer msDeployer;
    static MasterService()
    {
      Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var sections = cfg.Sections;
      var settings = (InitialSettingsConfigSection)sections["initialSettings"];
      var nodes = settings.ServiceNodesItems;
      int mCount = nodes[0].Count;
      int sCount = nodes[1].Count;


      msDeployer = new UserStorageAppDeployer(mCount, 0, nodes[0].FirstPort);

      msDeployer.DeployUserStorageApp();
      ConnectToSlaves(sCount, mCount, nodes[1].FirstPort, nodes[0].FirstPort);
    }

    /// <summary>
    /// Add user to service
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public int Add(UserContext user)
    {
      User u = ContextConverter.ContextToUser(user);
      msDeployer.masters[0].service.Add(u);
      return u.personalId;
    }

    /// <summary>
    /// Remove user from service
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool Remove(UserContext user)
    {
      User u = ContextConverter.ContextToUser(user);
      return msDeployer.masters[0].service.Remove(u);
    }

    /// <summary>
    /// Remove user by id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool RemoveById(int userId)
    {
      return msDeployer.masters[0].service.Remove(userId);
    }

    /// <summary>
    /// User search
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public IEnumerable<UserContext> Search(UserContext ctx)
    {
      IEnumerable<User> users = msDeployer.masters[0].service.GetAll();
      if (ctx.FirstName != null)
      {
        users = users.Where(u => u.FirstName == ctx.FirstName);
      }
      if (ctx.LastName != null)
      {
        users = users.Where(u => u.LastName == ctx.LastName);
      }
      if (ctx.DateOfBirth != default(DateTime)|| ctx.DateOfBirth!=null)
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
      User user = msDeployer.masters[0].service.FindById(id);
      return ContextConverter.UserToContext(user);
    }

    private static void ConnectToSlaves(int sCount, int mCount, int firstSlavePort, int firstMasterPort)
    {
      int[] slavesPorts = new int[sCount];
      int[] masterPorts = new int[mCount];
      for (int i = 0; i < sCount; i++)
      {
        slavesPorts[i] = firstSlavePort + i;
      }

      for (int i = 0; i < mCount; i++)
      {
        masterPorts[i] = firstMasterPort + i;
      }

      foreach (var master in msDeployer.masters)
      {
        master.SetSlavePorts(slavesPorts.Concat(masterPorts).ToArray());
      }
    }
  }
}
