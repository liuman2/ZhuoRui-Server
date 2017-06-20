DROP TABLE IF EXISTS `abroad_shareholder`;
CREATE TABLE `abroad_shareholder` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL COMMENT '订单ID',
  `history_id` int(11) DEFAULT NULL COMMENT '历史变更ID',
  `name` varchar(30) DEFAULT NULL COMMENT '姓名',
  `source` varchar(30) DEFAULT NULL COMMENT '源，reg_abroad,reg_internal',
  `gender` varchar(4) DEFAULT NULL COMMENT '性别',
  `cardNo` varchar(50) DEFAULT NULL COMMENT '身份证',
  `takes` float(255,2) DEFAULT NULL COMMENT '股份%',
  `type` varchar(10) DEFAULT NULL COMMENT '股东，董事',
  `changed_type` varchar(20) DEFAULT NULL COMMENT '变更类别 原始-original, 新进-new,退出-exit,股份调整-takes',
  `date_changed` datetime DEFAULT NULL COMMENT '变更日期',
  `memo` varchar(300) DEFAULT NULL COMMENT '备注',
  `attachment` varchar(300) DEFAULT NULL COMMENT 'url',
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('abroad_shareholder', '1');

DROP TABLE IF EXISTS `history_shareholder`;
CREATE TABLE `history_shareholder` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL COMMENT '订单ID',
  `history_id` int(11) DEFAULT NULL COMMENT '历史变更ID',
  `person_id` int(11) DEFAULT NULL COMMENT '来源人员id',
  `name` varchar(30) DEFAULT NULL COMMENT '姓名',
  `source` varchar(30) DEFAULT NULL COMMENT '源，reg_abroad,reg_internal',
  `gender` varchar(4) DEFAULT NULL COMMENT '性别',
  `cardNo` varchar(50) DEFAULT NULL COMMENT '身份证',
  `takes` float(255,2) DEFAULT NULL COMMENT '股份%',
  `type` varchar(10) DEFAULT NULL COMMENT '股东，董事',
  `changed_type` varchar(20) DEFAULT NULL COMMENT '变更类别 原始-original, 新进-new,退出-exit,股份调整-takes',
  `date_changed` datetime DEFAULT NULL COMMENT '变更日期',
  `memo` varchar(300) DEFAULT NULL COMMENT '备注',
  `attachment` varchar(300) DEFAULT NULL COMMENT 'url',
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('history_shareholder', '1');

DROP TABLE IF EXISTS `abroad_history`;
CREATE TABLE `abroad_history` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL COMMENT '订单ID',
  `name_cn` varchar(120) DEFAULT NULL,
  `name_en` varchar(120) DEFAULT NULL,
  `address` varchar(320) DEFAULT NULL,
  `reg_no` varchar(320) DEFAULT NULL,
  `others` varchar(400) DEFAULT NULL,
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('abroad_history', '1');
