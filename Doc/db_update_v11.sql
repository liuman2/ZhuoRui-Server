INSERT INTO `menu` VALUES ('72', '15', 'letter_insert', 'fa fa-file-o', '新增寄件', '38');
INSERT INTO `menu` VALUES ('73', '15', 'inbox_insert', 'fa fa-file-o', '新增收件', '39');


call AddColumnUnlessExists(Database(), 'settings', 'memo', 'varchar(50) DEFAULT NULL COMMENT "说明"');

ALTER TABLE mail ADD CONSTRAINT fk_mail_creator_id FOREIGN KEY (creator_id) REFERENCES member(id);
INSERT INTO `settings` VALUES ('4', 'JW_ID', null, null);
INSERT INTO `settings` VALUES ('5', 'GN_ID', null, null);
INSERT INTO `settings` VALUES ('6', 'JWSJ_ID', null, null);
INSERT INTO `settings` VALUES ('7', 'GNSJ_ID', null, null);
INSERT INTO `settings` VALUES ('8', 'SB_ID', null, null);
INSERT INTO `settings` VALUES ('9', 'ZL_ID', null, null);
INSERT INTO `settings` VALUES ('10', 'CW_ID', null, null);


