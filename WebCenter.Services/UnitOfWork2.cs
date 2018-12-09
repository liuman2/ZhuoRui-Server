﻿ 

 using WebCenter.IServices;

namespace WebCenter.Services
{
 
    public partial class UnitOfWork:IUnitOfWork
{  
	
  [Ninject.Inject]
	 public  Iabroad_historyService Iabroad_historyService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Iabroad_shareholderService Iabroad_shareholderService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IaccountingService IaccountingService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Iaccounting_itemService Iaccounting_itemService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Iaccounting_progressService Iaccounting_progressService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Iannual_examService Iannual_examService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IareaService IareaService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IattachmentService IattachmentService
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
	 public  Iaudit_bankService Iaudit_bankService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IbankService IbankService
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
	 public  Ibank_contactService Ibank_contactService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Ibusiness_bankService Ibusiness_bankService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IcontactService IcontactService
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
	 public  IhistoryService IhistoryService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Ihistory_shareholderService Ihistory_shareholderService
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
	 public  Iinternal_historyService Iinternal_historyService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Iinternal_shareholderService Iinternal_shareholderService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IleaveService IleaveService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IlectureService IlectureService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Ilecture_customerService Ilecture_customerService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  ImailService ImailService
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
	 public  ImenuService ImenuService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  InoticeService InoticeService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Iopen_bankService Iopen_bankService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IoperationService IoperationService
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
	 public  IpatentService IpatentService
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
	 public  IreceiptService IreceiptService
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
	 public  Ireg_internal_itemsService Ireg_internal_itemsService
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
	 public  Irole_memberService Irole_memberService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Irole_memuService Irole_memuService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Irole_operationService Irole_operationService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IscheduleService IscheduleService
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
	 public  IsettingService IsettingService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Isub_auditService Isub_auditService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IsupplierService IsupplierService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  Itax_recordService Itax_recordService
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
	
  [Ninject.Inject]
	 public  ItrademarkService ItrademarkService
    {
        get;
        set;
    } 
	
  [Ninject.Inject]
	 public  IwaitdealService IwaitdealService
    {
        get;
        set;
    } 
	
}
}