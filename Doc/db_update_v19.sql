call AddColumnUnlessExists(Database(), 'abroad_shareholder', 'position', 'varchar(30) DEFAULT NULL COMMENT "担任职务"');
call AddColumnUnlessExists(Database(), 'history_shareholder', 'position', 'varchar(30) DEFAULT NULL COMMENT "担任职务"');

DROP TABLE IF EXISTS `internal_shareholder`;
CREATE TABLE `internal_shareholder` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL COMMENT '订单ID',
  `history_id` int(11) DEFAULT NULL COMMENT '历史变更ID',
  `name` varchar(30) DEFAULT NULL COMMENT '姓名',
  `source` varchar(30) DEFAULT NULL COMMENT 'reg_internal',
  `gender` varchar(4) DEFAULT NULL COMMENT '性别',
  `cardNo` varchar(50) DEFAULT NULL COMMENT '身份证',
  `position` varchar(30) DEFAULT NULL COMMENT '担任职务',
  `takes` float(255,2) DEFAULT NULL COMMENT '股份%',
  `type` varchar(10) DEFAULT NULL COMMENT '股东，董事，监事',
  `changed_type` varchar(20) DEFAULT NULL COMMENT '变更类别 原始-original, 新进-new,退出-exit,股份调整-takes',
  `date_changed` datetime DEFAULT NULL COMMENT '变更日期',
  `memo` varchar(300) DEFAULT NULL COMMENT '备注',
  `attachment` varchar(300) DEFAULT NULL COMMENT 'url',
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('internal_shareholder', '1');

DROP TABLE IF EXISTS `internal_history`;
CREATE TABLE `internal_history` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL COMMENT '订单ID',
  `name_cn` varchar(120) DEFAULT NULL,
  `legal` varchar(120) DEFAULT NULL,
  `address` varchar(320) DEFAULT NULL,
  `reg_no` varchar(320) DEFAULT NULL,
  `director` varchar(320) DEFAULT NULL,
  `others` varchar(400) DEFAULT NULL,
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('internal_history', '1');




