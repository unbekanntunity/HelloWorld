import React, { Component, Children } from 'react';

import IconTwoLineItem from './IconTwoLineItem';
import TwoLineItem from './TwoLineItem';
import CenterLineItem from './CenterLineItem';

import './InputField.css';

class InputField extends Component {
    static IconTwoLineItem = IconTwoLineItem;
    static TwoLineItem = TwoLineItem;
    static CenterLineItem = CenterLineItem;

    renderItems = () => {
        let items = Children.map(this.props.children, (child, index) => {
            return React.cloneElement(child, {
                key: index,
                onClick: (event) => child.props.clickable ? this.props.onItemClick(index) : undefined
            });
        })

        return items;
    }

    render() {
        return (
            <div className="inputfield-container" style={{
                width: this.props.fill ? "100%" : "initial"
            } }>
                <div className={'inputbar-container-' + (this.props.design ?? 'm1')} style={{
                    width: this.props.width ?? '100%',
                    height: this.props.height ?? "100%"
                }}>
                    <div className="inputbar">
                        {this.props.icon !== undefined &&
                            <img src={this.props.icon} width={this.props.iconSize} height={this.props.iconSize} alt="" />
                        }
                        <input className={'inputbox-' + (this.props.design ?? 'm1')} name={this.props.propName} onChange={this.props.onChange} value={this.props.value}
                            style={{
                                borderBottom: this.props.showUnderline ? "initial" : "none"
                            }}

                            type={this.props.type}
                            placeholder={this.props.placeholder}>
                        </input>
                    </div>
                </div>
                {
                    this.renderItems()
                }
            </div>
        );
    }
}

export default InputField;