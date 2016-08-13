﻿ 

 using WebCenter.IServices;

namespace WebCenter.Services
{
 
    public partial class UnitOfWork:IUnitOfWork
{  
	
  [Ninject.Inject]
	 public  IareaService IareaService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IauditService IauditService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Ibank_accountService Ibank_accountService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IcustomerService IcustomerService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Icustomer_timelineService Icustomer_timelineService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IdictionaryService IdictionaryService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Idictionary_groupService Idictionary_groupService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IincomeService IincomeService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  ImemberService ImemberService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IorganizationService IorganizationService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IpositionService IpositionService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Ireg_abroadService Ireg_abroadService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Ireg_historyService Ireg_historyService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Ireg_internalService Ireg_internalService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Ireg_internal_historyService Ireg_internal_historyService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IroleService IroleService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IsequenceService IsequenceService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  ItimelineService ItimelineService
    {
        get;
        set;
    } 
	
}
}