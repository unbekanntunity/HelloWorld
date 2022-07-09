import React, { Component } from 'react';

import Tag from './Tag';
import ImageSlider from './ImageSlider';

import heart from '../images/heart.png';
import menuClosed from '../images/dots-vertical.png';
import menuOpened from '../images/dots-horizontal.png';
import DropDown from './DropDown';
import report from '../images/error.png';
import unfollow from '../images/minus.png';
import rightArrow from '../images/right-arrow2.png';
import share from '../images/share.png';

import VisibilitySensor from 'react-visibility-sensor';

import './Post.css';

class Post extends Component {
    state = {
        visibility: true,
        menuOpen: false,
    }

    handleMenu = () => {
        this.setState({
            menuOpen: !this.state.menuOpen,
        });
    }

    render() {
        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !this.props.creatorImage && this.props.onFirstAppear(this.props.keyProp)} >
                <div className="post-container" style={{
                    opacity: this.state.visibility ? 1 : 0.25,
                    transition: 'opacity 500ms linear',
                    width: this.props.width
                }}>
                    <div className="post-header">
                        <img className="post-owner-pic" src={this.props.creatorPic} alt="" width={40} height={40 } />
                        <div className="post-owner">
                            <h6 className="post-owner-name">{this.props.creatorName}</h6>
                            <p className="post-posted-at">24.06</p>
                        </div>
                        <div className="post-header-menu">
                            {
                                this.props.tags && this.props.tags.length > 0 && <Tag name={this.props.tags[0].name} />
                            }
                            <img src={heart} width={30} height={30} alt="" />
                            <p className="header-likes" style={{ marginLeft: 5 }}>100</p>
                            <DropDown toggleButton={{
                                icon: undefined,
                                arrowIconOpen: menuOpened,
                                arrowIconClose: menuClosed
                            }}
                                arrowIconSize={30} onHeaderClick={this.handleMenu}>
                                <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                                <DropDown.Item icon={unfollow} textColor="red" text="Unfollow" iconSize={30} onClick={this.props.onUnfollowClick} />
                                <DropDown.Item icon={rightArrow} text="Jump" iconSize={30} onClick={this.props.onRightArrowClick} />
                                <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                            </DropDown>
                        </div>
                    </div>

                    <p className="post-text">{this.props.text}</p>
                    {
                        this.props.images &&
                        <ImageSlider images={this.props.images} imageHeight={this.props.imageHeight} imageWidth={this.props.imageWidth} />
                    }
                </div>
            </VisibilitySensor>
        );
    }

}

export default Post;