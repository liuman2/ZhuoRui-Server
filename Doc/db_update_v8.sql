INSERT INTO `menu` VALUES ('40', '15', 'inbox', 'fa fa-inbox', '收件管理', '36');
INSERT INTO `menu` VALUES ('71', '15', 'letter_audit', 'fa fa-calculator', '信件审核', '37');

alter table reg_abroad modify column director varchar(100);
alter table reg_internal modify column director varchar(100);

call AddColumnUnlessExists(Database(), 'mail', 'paymode', 'varchar(10) DEFAULT NULL COMMENT "付款方式"');
