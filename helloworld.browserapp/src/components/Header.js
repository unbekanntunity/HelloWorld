import React, { Component } from 'react';

import heart from '../images/heart.png';
import menuClosed from '../images/dots-vertical.png';
import menuOpened from '../images/dots-horizontal.png';
import DropDown from './DropDown';
import report from '../images/error.png';
import save from '../images/bookmark.png';
import rightArrow from '../images/right-arrow2.png';
import share from '../images/share.png';
import user from '../images/user.png';

import Tag from './Tag';

import './Header.css';

class Header extends Component {
    render() {
        return (
            <div className="header-container">
                <img src={this.props.creatorImage} alt="" height={30} width={30} />
                <div className="header-middle">
                    <div className="header-text-container">
                        <p className="header-text">{this.props.title}</p>
                        {
                            this.props.createdAt &&
                            <p className="bold-gray-date header-date">{this.props.createdAt}</p>
                        }
                    </div>
                    <div className="header-tags">
                        {
                            this.props.tags &&
                            this.props.tags.map((item, index) =>
                                <Tag key={index} name={item.name} />
                            )
                        }
                    </div>
                </div>
                <span className="header-actions">
                    <div className="header-action">
                        <img src={user} width={30} height={30} alt="" />
                        <p className="header-likes">4</p>
                    </div>
                    <div className="header-action" style={{ marginRight: '0px' }}>
                        <img src={heart} width={30} height={30} alt="" />
                        <p className="header-likes">100</p>
                    </div>
                    <DropDown toggleButton={{
                        icon: undefined,
                        arrowIconOpen: menuOpened,
                        arrowIconClose: menuClosed
                    }}
                        arrowIconSize={30} onHeaderClick={this.handleMenu}>
                        <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                        <DropDown.Item icon={rightArrow} text="Jump" iconSize={30} onClick={this.props.onRightArrowClick} />
                        <DropDown.Item icon={save} text="Save" iconSize={30} onClick={this.props.saveClick} />
                        <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                    </DropDown>
                </span>
            </div>
        )
    }
}

export default Header;
