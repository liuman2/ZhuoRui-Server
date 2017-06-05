DROP TABLE IF EXISTS `abroad_shareholder`;
CREATE TABLE `abroad_shareholder` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL COMMENT '订单ID',
  `history_id` int(11) DEFAULT NULL COMMENT '历史变更ID',
  `name` varchar(30) DEFAULT NULL COMMENT '姓名',
  `gender` varchar(4) DEFAULT NULL COMMENT '性别',
  `cardNo` varchar(50) DEFAULT NULL COMMENT '身份证',
  `takes` float(255,2) DEFAULT NULL COMMENT '股份%',
  `type` varchar(10) DEFAULT NULL COMMENT '股东，董事',
  `changed_type` datetime DEFAULT NULL COMMENT '变更类别 原始-original, 新进-new,退出-exit,股份调整-takes',
  `date_changed` datetime DEFAULT NULL COMMENT '变更日期',
  `memo` varchar(300) DEFAULT NULL COMMENT '备注',
  `attachment` varchar(300) DEFAULT NULL COMMENT 'url',
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('abroad_shareholder', '1');
