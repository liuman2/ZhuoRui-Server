SET FOREIGN_KEY_CHECKS=0;

alter table reg_abroad add is_annual int(20) DEFAULT NULL;

alter table reg_internal add is_annual int(20) DEFAULT NULL;

alter table trademark add is_annual int(20) DEFAULT NULL;

alter table patent add is_annual int(20) DEFAULT NULL;

alter table audit add is_annual int(20) DEFAULT NULL;
